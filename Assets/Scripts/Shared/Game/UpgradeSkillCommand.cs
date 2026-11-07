using ArtifexMundi.Shared.Commands;

namespace ArtifexMundi.Shared.Game
{
    public sealed class UpgradeSkillCommand : IPlayerActionCommand
    {
        public UpgradeSkillCommand(string skillId) => SkillId = skillId;

        public string SkillId { get; }

        public string ActionKey => KeyFor(SkillId);

        public static string KeyFor(string skillId) => "skill.upgrade." + skillId;

        public string Validate(CommandContext context)
        {
            if (string.IsNullOrEmpty(SkillId))
                return "Skill id is required.";
            if (context.State.Player.Gold < context.Config.Economy.SkillUpgradeCost)
                return "Not enough gold.";
            return null;
        }

        public void Execute(CommandContext context)
        {
            var player = context.State.Player;
            player.Gold -= context.Config.Economy.SkillUpgradeCost;
            var level = player.GetSkillLevel(SkillId) + 1;
            player.SetSkillLevel(SkillId, level);

            context.Emit(new GoldChanged(player.Gold));
            context.Emit(new SkillLevelChanged(SkillId, level));
        }
    }
}
