using System;
using System.Collections.Generic;

namespace ArtifexMundi.Shared.Tutorial
{
    /// <summary>
    /// Player's tutorial progress. Persisted by the server, so killing the game restores the exact Entry.
    /// Public fields - plain serializable data mutated only by Shared Logic commands.
    /// </summary>
    [Serializable]
    public sealed class TutorialState
    {
        /// <summary>null when no sequence is running.</summary>
        public string ActiveSequenceId;

        public int ActiveEntryIndex;

        /// <summary>Sequences finished or skipped - never started again.</summary>
        public List<string> CompletedSequenceIds = new List<string>();

        public bool HasActiveSequence => !string.IsNullOrEmpty(ActiveSequenceId);

        public bool IsCompleted(string sequenceId) => CompletedSequenceIds.Contains(sequenceId);

        public TutorialState()
        {
        }

        public TutorialState(string activeSequenceId, int activeEntryIndex, List<string> completedSequenceIds)
        {
            ActiveSequenceId = activeSequenceId;
            ActiveEntryIndex = activeEntryIndex;
            CompletedSequenceIds = new List<string>(completedSequenceIds ?? new List<string>());
        }

        public TutorialState Clone()
        {
            return new TutorialState(ActiveSequenceId, ActiveEntryIndex, CompletedSequenceIds);
        }
    }
}