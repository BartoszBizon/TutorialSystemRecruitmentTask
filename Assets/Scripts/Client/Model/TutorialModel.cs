using ArtifexMundi.Client.Mvvm;
using ArtifexMundi.Shared.Commands;
using ArtifexMundi.Shared.Tutorial;

namespace ArtifexMundi.Client.Model
{
    /// <summary>
    /// Read-only facade over TutorialState + TutorialConfig. Turns tutorial changes into messages.
    /// Never mutates state - only Shared Logic does.
    /// </summary>
    public sealed class TutorialModel : IModel
    {
        private readonly IGameStateProvider _stateProvider;
        private readonly TutorialConfig _config;
        private readonly IMessenger _messenger;

        public TutorialModel(IGameStateProvider stateProvider, TutorialConfig config, IMessenger messenger)
        {
            _stateProvider = stateProvider;
            _config = config;
            _messenger = messenger;
        }

        public bool IsActive => TryGetActiveEntry(out _, out _);

        public int ActiveEntryIndex => _stateProvider.State.Tutorial.ActiveEntryIndex;

        public bool TryGetActiveEntry(out TutorialSequenceDefinition sequence, out TutorialEntryDefinition entry)
        {
            return TutorialRules.TryGetActiveEntry(_stateProvider.State, _config, out sequence, out entry);
        }

        public void Apply(IChange change)
        {
            if (change is TutorialEntryStarted || change is TutorialSequenceFinished)
                _messenger.Publish(new TutorialStepChangedMessage());
        }

        public void Refresh()
        {
            _messenger.Publish(new TutorialStepChangedMessage());
        }
    }
}
