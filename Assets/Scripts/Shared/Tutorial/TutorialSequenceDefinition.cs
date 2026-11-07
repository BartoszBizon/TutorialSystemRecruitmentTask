using System;
using System.Collections.Generic;
using ArtifexMundi.Shared.Tutorial.Triggers;

namespace ArtifexMundi.Shared.Tutorial
{
    /// <summary>
    /// Immutable, designer-authored description of a tutorial Sequence (logic part only - no texts/visuals).
    /// </summary>
    public sealed class TutorialSequenceDefinition
    {
        public TutorialSequenceDefinition(
            string id,
            IReadOnlyList<TutorialEntryDefinition> entries,
            bool isSkippable,
            ITutorialTrigger startTrigger = null)
        {
            if (string.IsNullOrEmpty(id)) throw new ArgumentException("Sequence id is required.", nameof(id));
            if (entries == null || entries.Count == 0) throw new ArgumentException($"Sequence '{id}' has no entries.", nameof(entries));

            Id = id;
            Entries = entries;
            IsSkippable = isSkippable;
            StartTrigger = startTrigger ?? AlwaysTrigger.Instance;
        }

        public string Id { get; }
        public IReadOnlyList<TutorialEntryDefinition> Entries { get; }

        /// <summary>Designer flag (Available / Unavailable) set per sequence.</summary>
        public bool IsSkippable { get; }

        public ITutorialTrigger StartTrigger { get; }

        /// <summary>Design rule: Skip applies only to sequences longer than a single Entry.</summary>
        public bool CanBeSkipped => IsSkippable && Entries.Count > 1;
    }
}
