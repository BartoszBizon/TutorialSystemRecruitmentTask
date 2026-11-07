namespace ArtifexMundi.Client.Model
{
    /// <summary>
    /// Active tutorial entry changed (started, advanced, finished, skipped or resynced).
    /// Signal only - subscribers read the current state from TutorialModel.
    /// </summary>
    public sealed class TutorialStepChangedMessage
    {
    }

    public sealed class PlayerChangedMessage
    {
    }
}
