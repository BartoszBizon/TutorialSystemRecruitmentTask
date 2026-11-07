using ArtifexMundi.Shared.State;
using ArtifexMundi.Shared.Tutorial.Triggers;

namespace ArtifexMundi.Shared.Game
{
    /// <summary>
    /// Game-specific tutorial trigger. Lives with the game, not in the tutorial module -
    /// the tutorial only defines ITutorialTrigger and knows nothing about gold.
    /// </summary>
    public sealed class GoldAtLeastTrigger : ITutorialTrigger
    {
        private readonly int _gold;

        public GoldAtLeastTrigger(int gold) => _gold = gold;

        public bool IsSatisfied(GameState state) => state.Player.Gold >= _gold;
    }
}
