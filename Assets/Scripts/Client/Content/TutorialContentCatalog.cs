using System;
using System.Collections.Generic;
using System.Linq;
using ArtifexMundi.Shared.Tutorial;

namespace ArtifexMundi.Client.Content
{
    /// <summary>
    /// Client-side tutorial content. Builds the logic config (TutorialConfig) for Shared Logic
    /// and serves presentation data (texts, sides, targets) for the ViewModel.
    /// On a real project the server gets the logic part exported to JSON from the same assets.
    /// </summary>
    public sealed class TutorialContentCatalog
    {
        private readonly IReadOnlyList<TutorialSequenceContent> _sequences;
        private readonly Dictionary<string, TutorialSequenceContent> _sequencesById;

        public TutorialContentCatalog(IReadOnlyList<TutorialSequenceContent> sequences)
        {
            var errors = TutorialContentValidator.Validate(sequences);
            if (errors.Count > 0)
                throw new ArgumentException("Invalid tutorial content:\n" + string.Join("\n", errors), nameof(sequences));

            _sequences = sequences;
            _sequencesById = sequences.ToDictionary(s => s.Id);
        }

        public TutorialConfig BuildConfig()
        {
            return new TutorialConfig(_sequences.Select(s => s.ToDefinition()).ToList());
        }

        public bool TryGetEntry(string sequenceId, int entryIndex, out TutorialSequenceContent sequence, out TutorialEntryContent entry)
        {
            entry = null;
            if (!_sequencesById.TryGetValue(sequenceId, out sequence) || entryIndex < 0 || entryIndex >= sequence.Entries.Count)
                return false;

            entry = sequence.Entries[entryIndex];
            return true;
        }
    }
}
