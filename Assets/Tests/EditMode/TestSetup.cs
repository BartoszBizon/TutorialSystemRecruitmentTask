using System.Linq;
using ArtifexMundi.Shared;
using ArtifexMundi.Shared.Commands;
using ArtifexMundi.Shared.Config;
using ArtifexMundi.Shared.Game;
using ArtifexMundi.Shared.State;
using ArtifexMundi.Shared.Tutorial;
using ArtifexMundi.Shared.Tutorial.Triggers;

namespace ArtifexMundi.Tests
{
    /// <summary>
    /// Small, explicit content for tests - independent of the demo asset.
    /// </summary>
    internal static class TestSetup
    {
        public const string Intro = "intro";          // 2 entries, skippable: dialog "V" -> collect reward (blocking)
        public const string Single = "single";        // 1 entry, skippable flag on, non-blocking; starts after intro
        public const string NoSkip = "noskip";        // 2 entries, skip flag off; never starts by itself
        public const int RewardGold = 100;
        public const int UpgradeCost = 100;

        public static CommandExecutor CreateExecutor() => SharedLogicFactory.CreateExecutor(CreateConfig());

        public static GameConfig CreateConfig()
        {
            var sequences = new[]
            {
                new TutorialSequenceDefinition(Intro, new[]
                {
                    new TutorialEntryDefinition("intro_dialog", isBlocking: true),
                    new TutorialEntryDefinition("intro_collect", isBlocking: true, CollectRewardCommand.Key)
                }, isSkippable: true),

                new TutorialSequenceDefinition(Single, new[]
                {
                    new TutorialEntryDefinition("single_collect", isBlocking: false, CollectRewardCommand.Key)
                }, isSkippable: true, new SequenceCompletedTrigger(Intro)),

                new TutorialSequenceDefinition(NoSkip, new[]
                {
                    new TutorialEntryDefinition("noskip_a", isBlocking: true),
                    new TutorialEntryDefinition("noskip_b", isBlocking: true)
                }, isSkippable: false, new GoldAtLeastTrigger(int.MaxValue))
            };

            return new GameConfig(new TutorialConfig(sequences.ToList()), new EconomyConfig(RewardGold, UpgradeCost));
        }

        /// <summary>State with the given sequence active at the given entry.</summary>
        public static GameState StateAt(string sequenceId, int entryIndex, int gold = 0)
        {
            var state = new GameState();
            state.Tutorial.ActiveSequenceId = sequenceId;
            state.Tutorial.ActiveEntryIndex = entryIndex;
            state.Player.Gold = gold;
            return state;
        }
    }
}
