using ArtifexMundi.Shared.Commands;

namespace ArtifexMundi.Shared.Tutorial.Commands
{
    public sealed class SkipTutorialSequenceCommand : ICommand
    {
        public SkipTutorialSequenceCommand(string sequenceId) => SequenceId = sequenceId;

        public string SequenceId { get; }

        public string Validate(CommandContext context)
        {
            if (context.State.Tutorial.ActiveSequenceId != SequenceId)
                return $"Sequence '{SequenceId}' is not active.";
            if (!context.Config.Tutorial.TryGetSequence(SequenceId, out var sequence))
                return $"Unknown sequence '{SequenceId}'.";
            if (!sequence.CanBeSkipped)
                return $"Sequence '{SequenceId}' cannot be skipped.";
            return null;
        }

        public void Execute(CommandContext context)
        {
            context.Config.Tutorial.TryGetSequence(SequenceId, out var sequence);
            TutorialRules.FinishSequence(context, sequence);
        }
    }
}
