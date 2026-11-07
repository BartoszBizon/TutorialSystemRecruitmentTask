using System;
using System.Collections.Generic;
using ArtifexMundi.Client.Model;
using ArtifexMundi.Client.Mvvm;

namespace ArtifexMundi.Client.ViewModel
{
    /// <summary>
    /// Game-specific rule: which objects the player should tap right now, in priority order.
    /// The first one present on screen gets the hint.
    /// </summary>
    public interface IIdleHintTargetProvider
    {
        IEnumerable<string> GetCandidateTargets();
    }

    /// <summary>
    /// Delayed hint (Podpowiedz opozniona): after X seconds of inactivity show a non-blocking indication
    /// on the object that should be tapped now. Any interaction hides it and resets the timeout.
    /// Client-only and not persisted - it is not part of tutorial progress, so it never goes through Shared Logic.
    /// Disabled while a tutorial sequence is active.
    /// </summary>
    public sealed class IdleHintViewModel : ViewModelBase, IDisposable
    {
        private readonly TutorialModel _tutorial;
        private readonly IIdleHintTargetProvider _targetProvider;
        private readonly ITutorialTargetRegistry _targets;
        private readonly float _timeoutSeconds;
        private readonly IDisposable _subscription;

        private float _idleTime;
        private string _hintTargetId;

        public IdleHintViewModel(
            TutorialModel tutorial,
            IIdleHintTargetProvider targetProvider,
            ITutorialTargetRegistry targets,
            IMessenger messenger,
            float timeoutSeconds)
        {
            _tutorial = tutorial;
            _targetProvider = targetProvider;
            _targets = targets;
            _timeoutSeconds = timeoutSeconds;

            InteractionCommand = new RelayCommand(ResetTimer);
            _subscription = messenger.Subscribe<TutorialStepChangedMessage>(_ => ResetTimer());
            _targets.TargetsChanged += OnTargetsChanged;
        }

        /// <summary>null = no hint visible.</summary>
        public string HintTargetId { get => _hintTargetId; private set => SetProperty(ref _hintTargetId, value); }

        /// <summary>Raised by the View on any player input.</summary>
        public RelayCommand InteractionCommand { get; }

        public void Tick(float deltaTime)
        {
            if (_tutorial.IsActive)
            {
                ResetTimer();
                return;
            }

            _idleTime += deltaTime;
            if (HintTargetId == null && _idleTime >= _timeoutSeconds)
                HintTargetId = FindTarget();
        }

        public void Dispose()
        {
            _subscription.Dispose();
            _targets.TargetsChanged -= OnTargetsChanged;
        }

        private void ResetTimer()
        {
            _idleTime = 0f;
            HintTargetId = null;
        }

        private void OnTargetsChanged()
        {
            // Hinted object left the screen - hide; Tick picks a new target on the next frame.
            if (HintTargetId != null && !_targets.IsAvailable(HintTargetId))
                HintTargetId = null;
        }

        private string FindTarget()
        {
            foreach (var targetId in _targetProvider.GetCandidateTargets())
            {
                if (_targets.IsAvailable(targetId))
                    return targetId;
            }

            return null;
        }
    }
}
