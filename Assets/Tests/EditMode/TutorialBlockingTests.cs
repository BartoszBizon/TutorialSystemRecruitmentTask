using ArtifexMundi.Shared.Game;
using NUnit.Framework;

namespace ArtifexMundi.Tests
{
    /// <summary>Blocking entry: only the action completing it is allowed - enforced in Shared Logic, so on the server too.</summary>
    public sealed class TutorialBlockingTests
    {
        [Test]
        public void BlockingEntry_OtherPlayerAction_IsRejectedWithoutStateChange()
        {
            var state = TestSetup.StateAt(TestSetup.Intro, 1, gold: TestSetup.UpgradeCost);

            var result = TestSetup.CreateExecutor().Execute(state, new UpgradeSkillCommand("strength"));

            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(TestSetup.UpgradeCost, state.Player.Gold);
            Assert.AreEqual(0, state.Player.GetSkillLevel("strength"));
        }

        [Test]
        public void BlockingEntry_CompletingAction_IsAllowed()
        {
            var state = TestSetup.StateAt(TestSetup.Intro, 1);

            var result = TestSetup.CreateExecutor().Execute(state, new CollectRewardCommand());

            Assert.IsTrue(result.IsSuccess);
        }

        [Test]
        public void NonBlockingEntry_OtherPlayerAction_IsAllowedAndDoesNotAdvanceTutorial()
        {
            var state = TestSetup.StateAt(TestSetup.Single, 0, gold: TestSetup.UpgradeCost);

            var result = TestSetup.CreateExecutor().Execute(state, new UpgradeSkillCommand("strength"));

            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(TestSetup.Single, state.Tutorial.ActiveSequenceId);
            Assert.AreEqual(0, state.Tutorial.ActiveEntryIndex);
        }
    }
}
