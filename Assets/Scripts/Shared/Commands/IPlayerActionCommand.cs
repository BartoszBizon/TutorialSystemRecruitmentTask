namespace ArtifexMundi.Shared.Commands
{
    /// <summary>
    /// Gameplay command triggered by the player (tap on an object).
    /// ActionKey identifies the action, so a tutorial entry can be completed by it - or block it.
    /// </summary>
    public interface IPlayerActionCommand : ICommand
    {
        string ActionKey { get; }
    }
}
