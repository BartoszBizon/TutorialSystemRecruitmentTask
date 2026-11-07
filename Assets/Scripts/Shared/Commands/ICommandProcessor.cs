namespace ArtifexMundi.Shared.Commands
{
    /// <summary>
    /// Cross-cutting rule validated before every command (e.g. tutorial blocking other actions).
    /// Returns null when the command is allowed.
    /// </summary>
    public interface ICommandPreProcessor
    {
        string Validate(CommandContext context, ICommand command);
    }

    /// <summary>
    /// Cross-cutting reaction executed after every successful command, in the same transaction
    /// (e.g. tutorial progression).
    /// </summary>
    public interface ICommandPostProcessor
    {
        void OnExecuted(CommandContext context, ICommand command);
    }
}
