using System.Collections.Generic;
using ArtifexMundi.Client.Model;
using ArtifexMundi.Client.ViewModel;
using ArtifexMundi.Demo.Content;

namespace ArtifexMundi.Demo.Hud
{
    /// <summary>Demo rule: can afford an upgrade -> point to the upgrade path, otherwise to the reward.</summary>
    public sealed class DemoIdleHintTargetProvider : IIdleHintTargetProvider
    {
        private readonly PlayerModel _player;

        public DemoIdleHintTargetProvider(PlayerModel player) => _player = player;

        public IEnumerable<string> GetCandidateTargets()
        {
            if (_player.Gold >= _player.Economy.SkillUpgradeCost)
            {
                yield return DemoIds.UpgradeStrengthButton;
                yield return DemoIds.SkillsButton;
            }

            yield return DemoIds.RewardButton;
        }
    }
}
