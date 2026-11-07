using ArtifexMundi.Shared.Commands;

namespace ArtifexMundi.Shared.Tutorial.Processors
{
    /// <summary>
    /// Blocking entry: no player action is allowed except the one completing the entry.
    /// The client blocks input visually; this makes the rule authoritative on the server too.
    /// </summary>
    public sealed class TutorialBlockingPreProcessor : ICommandPreProcessor
    {
        public string Validate(CommandContext context, ICommand command)
        {
            if (!(command is IPlayerActionCommand action))
                return null;

            if (!TutorialRules.TryGetActiveEntry(context.State, context.Config.Tutorial, out _, out var entry))
                return null;

            if (!entry.IsBlocking || entry.CompletionActionKey == action.ActionKey)
                return null;

            return $"Action '{action.ActionKey}' is blocked by tutorial entry '{entry.Id}'.";
        }
    }
}
