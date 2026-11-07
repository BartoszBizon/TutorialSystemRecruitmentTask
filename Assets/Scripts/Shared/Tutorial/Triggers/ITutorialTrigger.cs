using ArtifexMundi.Shared.State;

namespace ArtifexMundi.Shared.Tutorial.Triggers
{
    /// <summary>
    /// Condition to start a sequence. Depends on state only (not on "what just happened"),
    /// so it is deterministic and can be re-evaluated at any time - e.g. after restart.
    /// </summary>
    public interface ITutorialTrigger
    {
        bool IsSatisfied(GameState state);
    }
}
