namespace ArtifexMundi.Shared.Commands
{
    /// <summary>
    /// Command executed by Shared Logic - identically on client (prediction) and server (authority).
    /// Validate must not mutate state; Execute is called only after successful validation and must not fail.
    /// </summary>
    public interface ICommand
    {
        string Validate(CommandContext context);
        void Execute(CommandContext context);
    }
}
