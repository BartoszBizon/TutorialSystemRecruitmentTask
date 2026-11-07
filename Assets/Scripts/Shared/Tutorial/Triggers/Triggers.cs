using System.Collections.Generic;
using System.Linq;
using ArtifexMundi.Shared.State;

namespace ArtifexMundi.Shared.Tutorial.Triggers
{
    public sealed class AlwaysTrigger : ITutorialTrigger
    {
        public static readonly AlwaysTrigger Instance = new AlwaysTrigger();

        public bool IsSatisfied(GameState state) => true;
    }

    public sealed class SequenceCompletedTrigger : ITutorialTrigger
    {
        private readonly string _sequenceId;

        public SequenceCompletedTrigger(string sequenceId) => _sequenceId = sequenceId;

        public bool IsSatisfied(GameState state) => state.Tutorial.IsCompleted(_sequenceId);
    }

    public sealed class AllOfTrigger : ITutorialTrigger
    {
        private readonly IReadOnlyList<ITutorialTrigger> _triggers;

        public AllOfTrigger(params ITutorialTrigger[] triggers) => _triggers = triggers;

        public bool IsSatisfied(GameState state) => _triggers.All(t => t.IsSatisfied(state));
    }
}
