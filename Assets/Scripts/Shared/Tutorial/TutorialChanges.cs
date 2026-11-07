using ArtifexMundi.Shared.Commands;

namespace ArtifexMundi.Shared.Tutorial
{
    /// <summary>Sequence started or moved to the next entry.</summary>
    public sealed class TutorialEntryStarted : IChange
    {
        public TutorialEntryStarted(string sequenceId, int entryIndex)
        {
            SequenceId = sequenceId;
            EntryIndex = entryIndex;
        }

        public string SequenceId { get; }
        public int EntryIndex { get; }
    }

    public sealed class TutorialSequenceFinished : IChange
    {
        public TutorialSequenceFinished(string sequenceId) => SequenceId = sequenceId;

        public string SequenceId { get; }
    }
}
