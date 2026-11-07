using System.Collections.Generic;

namespace ArtifexMundi.Client.Content
{
    /// <summary>
    /// Catches content mistakes that would leave the player stuck in a tutorial.
    /// Runs when content is loaded (fail fast) and in the editor (OnValidate).
    /// </summary>
    public static class TutorialContentValidator
    {
        public static List<string> Validate(IReadOnlyList<TutorialSequenceContent> sequences)
        {
            var errors = new List<string>();
            var ids = new HashSet<string>();

            foreach (var sequence in sequences)
            {
                if (string.IsNullOrEmpty(sequence.Id))
                {
                    errors.Add("Sequence without id.");
                    continue;
                }

                if (!ids.Add(sequence.Id))
                    errors.Add($"Duplicated sequence id '{sequence.Id}'.");

                if (sequence.Entries.Count == 0)
                    errors.Add($"Sequence '{sequence.Id}' has no entries.");

                for (var i = 0; i < sequence.Entries.Count; i++)
                    ValidateEntry(sequence.Entries[i], $"{sequence.Id}[{i}]", errors);
            }

            foreach (var sequence in sequences)
            foreach (var condition in sequence.StartConditions)
            {
                if (condition.Type == TutorialTriggerType.SequenceCompleted && !ids.Contains(condition.SequenceId ?? string.Empty))
                    errors.Add($"Sequence '{sequence.Id}' waits for unknown sequence '{condition.SequenceId}'.");
            }

            return errors;
        }

        private static void ValidateEntry(TutorialEntryContent entry, string path, List<string> errors)
        {
            var completedByAction = !string.IsNullOrEmpty(entry.CompletionActionKey);

            // Nothing to tap - the entry can never be completed.
            if (!entry.Dialog.Enabled && !entry.Indication.Enabled)
                errors.Add($"{path}: entry has neither a dialog nor an indication.");

            // "V" would send CompleteTutorialEntryCommand, which Shared Logic rejects for action-completed entries.
            if (entry.IsDialogOnly && completedByAction)
                errors.Add($"{path}: dialog-only entry cannot be completed by action '{entry.CompletionActionKey}' - add an indication or clear the key.");

            if (entry.Indication.Enabled && string.IsNullOrEmpty(entry.Indication.TargetId))
                errors.Add($"{path}: indication without TargetId.");

            // Blocking entry blocks gameplay commands in Shared Logic even when its target is off screen,
            // so the client must always be able to bring the target back (e.g. after restart).
            if (entry.IsBlocking && entry.Indication.Enabled && string.IsNullOrEmpty(entry.Indication.ScreenId))
                errors.Add($"{path}: blocking indication requires ScreenId (target may be off screen after restart).");
        }
    }
}
