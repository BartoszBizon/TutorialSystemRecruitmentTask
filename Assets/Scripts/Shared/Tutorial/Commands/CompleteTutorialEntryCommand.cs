using ArtifexMundi.Shared.Commands;

namespace ArtifexMundi.Shared.Tutorial.Commands
{
    /// <summary>
    /// Completes an entry that is not tied to a gameplay action (dialog "V", UI-only tap like opening a panel).
    /// SequenceId + EntryIndex make it idempotent: a duplicated / late request for an old entry is rejected
    /// instead of completing the current one.
    /// </summary>
    public sealed class CompleteTutorialEntryCommand : ICommand
    {
        public CompleteTutorialEntryCommand(string sequenceId, int entryIndex)
        {
            SequenceId = sequenceId;
            EntryIndex = entryIndex;
        }

        public string SequenceId { get; }
        public int EntryIndex { get; }

        public string Validate(CommandContext context)
        {
            if (!TutorialRules.TryGetActiveEntry(context.State, context.Config.Tutorial, out var sequence, out var entry))
                return "No active tutorial entry.";
            if (sequence.Id != SequenceId || context.State.Tutorial.ActiveEntryIndex != EntryIndex)
                return $"Entry {SequenceId}[{EntryIndex}] is not active.";
            if (entry.IsCompletedByAction)
                return $"Entry '{entry.Id}' can be completed only by action '{entry.CompletionActionKey}'.";
            return null;
        }

        public void Execute(CommandContext context)
        {
            context.Config.Tutorial.TryGetSequence(SequenceId, out var sequence);
            TutorialRules.AdvanceEntry(context, sequence);
        }
    }
}
