using ArtifexMundi.Shared.Commands;

namespace ArtifexMundi.Shared.Game
{
    public sealed class CollectRewardCommand : IPlayerActionCommand
    {
        public const string Key = "reward.collect";

        public string ActionKey => Key;

        public string Validate(CommandContext context) => null;

        public void Execute(CommandContext context)
        {
            var player = context.State.Player;
            player.Gold += context.Config.Economy.RewardGold;
            context.Emit(new GoldChanged(player.Gold));
        }
    }
}
