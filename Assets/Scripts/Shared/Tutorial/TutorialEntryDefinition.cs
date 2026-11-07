namespace ArtifexMundi.Shared.Tutorial
{
    public sealed class TutorialEntryDefinition
    {
        public string Id { get; }
        public bool IsBlocking { get; }

        /// <summary>
        /// Action (IPlayerActionCommand.ActionKey) that completes this entry.
        /// null = completed manually from UI (dialog "V", UI-only tap) via CompleteTutorialEntryCommand.
        /// </summary>
        public string CompletionActionKey { get; }

        public bool IsCompletedByAction => CompletionActionKey != null;

        public TutorialEntryDefinition(string id, bool isBlocking, string completionActionKey = null)
        {
            Id = id;
            IsBlocking = isBlocking;
            CompletionActionKey = completionActionKey;
        }
    }
}
