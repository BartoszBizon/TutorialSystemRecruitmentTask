using ArtifexMundi.Shared.Commands;
using ArtifexMundi.Shared.State;

namespace ArtifexMundi.Shared.Tutorial
{
    /// <summary>
    /// The only place that mutates TutorialState. Used by tutorial commands and processors.
    /// </summary>
    public static class TutorialRules
    {
        public static bool TryGetActiveEntry(
            GameState state,
            TutorialConfig config,
            out TutorialSequenceDefinition sequence,
            out TutorialEntryDefinition entry)
        {
            sequence = null;
            entry = null;

            var tutorial = state.Tutorial;
            if (!tutorial.HasActiveSequence || !config.TryGetSequence(tutorial.ActiveSequenceId, out sequence))
                return false;

            if (tutorial.ActiveEntryIndex < 0 || tutorial.ActiveEntryIndex >= sequence.Entries.Count)
                return false;

            entry = sequence.Entries[tutorial.ActiveEntryIndex];
            return true;
        }

        public static void StartSequence(CommandContext context, TutorialSequenceDefinition sequence)
        {
            var tutorial = context.State.Tutorial;
            tutorial.ActiveSequenceId = sequence.Id;
            tutorial.ActiveEntryIndex = 0;
            context.Emit(new TutorialEntryStarted(sequence.Id, 0));
        }

        /// <summary>Moves to the next entry, or finishes the sequence after the last one.</summary>
        public static void AdvanceEntry(CommandContext context, TutorialSequenceDefinition sequence)
        {
            var tutorial = context.State.Tutorial;
            var nextIndex = tutorial.ActiveEntryIndex + 1;

            if (nextIndex >= sequence.Entries.Count)
            {
                FinishSequence(context, sequence);
                return;
            }

            tutorial.ActiveEntryIndex = nextIndex;
            context.Emit(new TutorialEntryStarted(sequence.Id, nextIndex));
        }

        public static void FinishSequence(CommandContext context, TutorialSequenceDefinition sequence)
        {
            var tutorial = context.State.Tutorial;
            tutorial.ActiveSequenceId = null;
            tutorial.ActiveEntryIndex = 0;
            if (!tutorial.IsCompleted(sequence.Id))
                tutorial.CompletedSequenceIds.Add(sequence.Id);

            context.Emit(new TutorialSequenceFinished(sequence.Id));
        }

        /// <summary>Starts the first (config order = priority) not completed sequence whose trigger is satisfied.</summary>
        public static void TryStartNextSequence(CommandContext context)
        {
            var tutorial = context.State.Tutorial;
            if (tutorial.HasActiveSequence)
                return;

            foreach (var sequence in context.Config.Tutorial.Sequences)
            {
                if (tutorial.IsCompleted(sequence.Id) || !sequence.StartTrigger.IsSatisfied(context.State))
                    continue;

                StartSequence(context, sequence);
                return;
            }
        }
    }
}
