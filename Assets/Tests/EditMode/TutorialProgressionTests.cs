using ArtifexMundi.Shared.Game;
using ArtifexMundi.Shared.Session;
using ArtifexMundi.Shared.State;
using ArtifexMundi.Shared.Tutorial;
using ArtifexMundi.Shared.Tutorial.Commands;
using NUnit.Framework;

namespace ArtifexMundi.Tests
{
    public sealed class TutorialProgressionTests
    {
        [Test]
        public void StartSession_NewPlayer_StartsFirstSequenceWithSatisfiedTrigger()
        {
            var state = new GameState();

            var result = TestSetup.CreateExecutor().Execute(state, new StartSessionCommand());

            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(TestSetup.Intro, state.Tutorial.ActiveSequenceId);
            Assert.AreEqual(0, state.Tutorial.ActiveEntryIndex);
        }

        [Test]
        public void CompleteEntry_ActiveUiEntry_AdvancesToNextEntry()
        {
            var state = TestSetup.StateAt(TestSetup.Intro, 0);

            var result = TestSetup.CreateExecutor().Execute(state, new CompleteTutorialEntryCommand(TestSetup.Intro, 0));

            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(1, state.Tutorial.ActiveEntryIndex);
        }

        [Test]
        public void CompleteEntry_DuplicatedRequest_IsRejectedAndDoesNotSkipEntry()
        {
            var executor = TestSetup.CreateExecutor();
            var state = TestSetup.StateAt(TestSetup.Intro, 0);

            executor.Execute(state, new CompleteTutorialEntryCommand(TestSetup.Intro, 0));
            var duplicate = executor.Execute(state, new CompleteTutorialEntryCommand(TestSetup.Intro, 0));

            Assert.IsFalse(duplicate.IsSuccess);
            Assert.AreEqual(1, state.Tutorial.ActiveEntryIndex);
        }

        [Test]
        public void CompleteEntry_EntryCompletedByAction_IsRejected()
        {
            var state = TestSetup.StateAt(TestSetup.Intro, 1);

            var result = TestSetup.CreateExecutor().Execute(state, new CompleteTutorialEntryCommand(TestSetup.Intro, 1));

            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(TestSetup.Intro, state.Tutorial.ActiveSequenceId);
        }

        [Test]
        public void PlayerAction_ExpectedByEntry_ExecutesActionAndAdvancesTutorialInSameCommand()
        {
            var state = TestSetup.StateAt(TestSetup.Intro, 1);

            var result = TestSetup.CreateExecutor().Execute(state, new CollectRewardCommand());

            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(TestSetup.RewardGold, state.Player.Gold);
            Assert.IsTrue(state.Tutorial.IsCompleted(TestSetup.Intro));
        }

        [Test]
        public void LastEntryCompleted_FinishesSequenceAndStartsNextTriggeredOne()
        {
            var state = TestSetup.StateAt(TestSetup.Intro, 1);

            var result = TestSetup.CreateExecutor().Execute(state, new CollectRewardCommand());

            Assert.IsTrue(state.Tutorial.IsCompleted(TestSetup.Intro));
            Assert.AreEqual(TestSetup.Single, state.Tutorial.ActiveSequenceId);
            // Client gets both changes from one command: old sequence finished, new one started.
            Assert.IsTrue(result.Changes[result.Changes.Count - 2] is TutorialSequenceFinished);
            Assert.IsTrue(result.Changes[result.Changes.Count - 1] is TutorialEntryStarted);
        }
    }
}
