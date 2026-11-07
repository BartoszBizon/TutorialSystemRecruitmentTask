using ArtifexMundi.Shared.Commands;

namespace ArtifexMundi.Shared.Tutorial.Processors
{
    /// <summary>
    /// Runs after every successful command, in the same transaction:
    /// 1. Command is the action expected by the active entry -> advance the tutorial
    ///    ("tap on the indicated object = object's own logic + next Entry").
    /// 2. No sequence active -> start the next one whose trigger is satisfied.
    /// </summary>
    public sealed class TutorialProgressionPostProcessor : ICommandPostProcessor
    {
        public void OnExecuted(CommandContext context, ICommand command)
        {
            if (command is IPlayerActionCommand action
                && TutorialRules.TryGetActiveEntry(context.State, context.Config.Tutorial, out var sequence, out var entry)
                && entry.CompletionActionKey == action.ActionKey)
            {
                TutorialRules.AdvanceEntry(context, sequence);
            }

            TutorialRules.TryStartNextSequence(context);
        }
    }
}
