namespace ArtifexMundi.Shared.Game
{
    public sealed class EconomyConfig
    {
        public EconomyConfig(int rewardGold, int skillUpgradeCost)
        {
            RewardGold = rewardGold;
            SkillUpgradeCost = skillUpgradeCost;
        }

        public int RewardGold { get; }
        public int SkillUpgradeCost { get; }
    }
}
