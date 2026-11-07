using System;
using ArtifexMundi.Client.Commands;
using ArtifexMundi.Client.Content;
using ArtifexMundi.Client.Model;
using ArtifexMundi.Client.Mvvm;
using ArtifexMundi.Shared.Tutorial;
using ArtifexMundi.Shared.Tutorial.Commands;

namespace ArtifexMundi.Client.ViewModel
{
    /// <summary>
    /// Presentation state of the active tutorial entry (Dialog / Indication / Skip) + UI actions.
    /// Refreshed on TutorialStepChangedMessage and when targets appear/disappear on screen.
    /// </summary>
    public sealed class TutorialViewModel : ViewModelBase, IDisposable
    {
        private readonly TutorialModel _model;
        private readonly TutorialContentCatalog _content;
        private readonly ICommandSender _commandSender;
        private readonly ITutorialTargetRegistry _targets;
        private readonly IScreenNavigator _navigator;
        private readonly IDisposable _subscription;

        private string _sequenceId;
        private int _entryIndex;
        private TutorialEntryDefinition _entry;

        private bool _isDialogVisible;
        private string _dialogSpeaker;
        private string _dialogText;
        private DialogSide _dialogSide;
        private int _dialogDots;
        private bool _isContinueVisible;
        private bool _isIndicationVisible;
        private string _indicationTargetId;
        private HighlightStyle _highlightStyle;
        private bool _showPointer;
        private bool _isInputBlocked;
        private bool _isSkipVisible;

        public TutorialViewModel(
            TutorialModel model,
            TutorialContentCatalog content,
            ICommandSender commandSender,
            ITutorialTargetRegistry targets,
            IScreenNavigator navigator,
            IMessenger messenger)
        {
            _model = model;
            _content = content;
            _commandSender = commandSender;
            _targets = targets;
            _navigator = navigator;

            ContinueCommand = new RelayCommand(Continue, () => IsContinueVisible);
            SkipCommand = new RelayCommand(Skip, () => IsSkipVisible);
            TargetTappedCommand = new RelayCommand<string>(OnTargetTapped);

            _subscription = messenger.Subscribe<TutorialStepChangedMessage>(_ => Refresh());
            _targets.TargetsChanged += Refresh;
            Refresh();
        }

        public bool IsDialogVisible { get => _isDialogVisible; private set => SetProperty(ref _isDialogVisible, value); }
        public string DialogSpeaker { get => _dialogSpeaker; private set => SetProperty(ref _dialogSpeaker, value); }
        public string DialogText { get => _dialogText; private set => SetProperty(ref _dialogText, value); }
        public DialogSide DialogSide { get => _dialogSide; private set => SetProperty(ref _dialogSide, value); }

        /// <summary>Dots before "V": how many more dialog-only entries follow before the player has to act.</summary>
        public int DialogDots { get => _dialogDots; private set => SetProperty(ref _dialogDots, value); }

        /// <summary>"V" button - dialog without indication is continued by tapping it.</summary>
        public bool IsContinueVisible { get => _isContinueVisible; private set => SetProperty(ref _isContinueVisible, value); }

        public bool IsIndicationVisible { get => _isIndicationVisible; private set => SetProperty(ref _isIndicationVisible, value); }
        public string IndicationTargetId { get => _indicationTargetId; private set => SetProperty(ref _indicationTargetId, value); }
        public HighlightStyle HighlightStyle { get => _highlightStyle; private set => SetProperty(ref _highlightStyle, value); }
        public bool ShowPointer { get => _showPointer; private set => SetProperty(ref _showPointer, value); }

        /// <summary>Blocking entry: everything except the target (or the "V" button) ignores input.</summary>
        public bool IsInputBlocked { get => _isInputBlocked; private set => SetProperty(ref _isInputBlocked, value); }

        public bool IsSkipVisible { get => _isSkipVisible; private set => SetProperty(ref _isSkipVisible, value); }

        public RelayCommand ContinueCommand { get; }
        public RelayCommand SkipCommand { get; }

        /// <summary>Raised by the View when a TutorialAnchor is tapped.</summary>
        public RelayCommand<string> TargetTappedCommand { get; }

        public void Dispose()
        {
            _subscription.Dispose();
            _targets.TargetsChanged -= Refresh;
        }

        private void Refresh()
        {
            if (!_model.TryGetActiveEntry(out var sequence, out var entry)
                || !_content.TryGetEntry(sequence.Id, _model.ActiveEntryIndex, out var sequenceContent, out var entryContent))
            {
                Clear();
                return;
            }

            _sequenceId = sequence.Id;
            _entryIndex = _model.ActiveEntryIndex;
            _entry = entry;

            var dialog = entryContent.Dialog;
            var indication = entryContent.Indication;
            var targetAvailable = indication.Enabled && _targets.IsAvailable(indication.TargetId);

            // Blocking target not on screen (e.g. restart inside a panel) - bring its screen up.
            if (indication.Enabled && !targetAvailable && entry.IsBlocking && !string.IsNullOrEmpty(indication.ScreenId))
            {
                _navigator.Show(indication.ScreenId);
                // Opening the screen registers its anchors (and may re-enter Refresh) - read availability again.
                targetAvailable = _targets.IsAvailable(indication.TargetId);
            }

            // Entry with an indication is shown only while its target is on screen:
            // a non-blocking entry disappears when the player leaves the screen and comes back with it.
            var isPresentable = !indication.Enabled || targetAvailable;

            IsDialogVisible = dialog.Enabled && isPresentable;
            DialogSpeaker = dialog.Speaker;
            DialogText = dialog.Text;
            DialogSide = dialog.Side;
            DialogDots = CountFollowingDialogOnlyEntries(sequenceContent, _entryIndex);
            IsContinueVisible = IsDialogVisible && !indication.Enabled;

            IndicationTargetId = indication.Enabled ? indication.TargetId : null;
            HighlightStyle = indication.Highlight;
            ShowPointer = indication.ShowPointer;
            IsIndicationVisible = targetAvailable;

            IsInputBlocked = entry.IsBlocking && isPresentable;
            IsSkipVisible = sequence.CanBeSkipped && isPresentable;

            ContinueCommand.RaiseCanExecuteChanged();
            SkipCommand.RaiseCanExecuteChanged();
        }

        private void Clear()
        {
            _sequenceId = null;
            _entry = null;
            IsDialogVisible = false;
            IsContinueVisible = false;
            DialogDots = 0;
            IsIndicationVisible = false;
            IndicationTargetId = null;
            IsInputBlocked = false;
            IsSkipVisible = false;
            ContinueCommand.RaiseCanExecuteChanged();
            SkipCommand.RaiseCanExecuteChanged();
        }

        private static int CountFollowingDialogOnlyEntries(TutorialSequenceContent sequence, int entryIndex)
        {
            var count = 0;
            for (var i = entryIndex + 1; i < sequence.Entries.Count && sequence.Entries[i].IsDialogOnly; i++)
                count++;
            return count;
        }

        private void Continue()
        {
            _commandSender.Send(new CompleteTutorialEntryCommand(_sequenceId, _entryIndex));
        }

        private void Skip()
        {
            _commandSender.Send(new SkipTutorialSequenceCommand(_sequenceId));
        }

        private void OnTargetTapped(string targetId)
        {
            if (_entry == null || targetId != IndicationTargetId)
                return;

            // Entries completed by a gameplay action advance in Shared Logic together with that action
            // (the tapped button sends its own command). Only UI-only entries are completed here.
            if (!_entry.IsCompletedByAction)
                _commandSender.Send(new CompleteTutorialEntryCommand(_sequenceId, _entryIndex));
        }
    }
}
