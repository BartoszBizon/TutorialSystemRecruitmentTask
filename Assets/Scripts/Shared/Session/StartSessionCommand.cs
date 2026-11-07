using ArtifexMundi.Shared.Commands;

namespace ArtifexMundi.Shared.Session
{
    /// <summary>
    /// Sent once after login. Does nothing by itself - post-processors react to it
    /// (e.g. tutorial starts the first sequence whose trigger is satisfied).
    /// </summary>
    public sealed class StartSessionCommand : ICommand
    {
        public string Validate(CommandContext context) => null;

        public void Execute(CommandContext context)
        {
        }
    }
}
