using ArtifexMundi.Shared.Tutorial.Commands;
using NUnit.Framework;

namespace ArtifexMundi.Tests
{
    /// <summary>Skip rules from the feature design.</summary>
    public sealed class TutorialSkipTests
    {
        [Test]
        public void Skip_MultiEntrySequenceWithFlag_FinishesWholeSequence()
        {
            var state = TestSetup.StateAt(TestSetup.Intro, 0);

            var result = TestSetup.CreateExecutor().Execute(state, new SkipTutorialSequenceCommand(TestSetup.Intro));

            Assert.IsTrue(result.IsSuccess);
            Assert.IsTrue(state.Tutorial.IsCompleted(TestSetup.Intro));
            Assert.AreNotEqual(TestSetup.Intro, state.Tutorial.ActiveSequenceId);
        }

        [Test]
        public void Skip_SingleEntrySequence_IsRejectedEvenWithFlagOn()
        {
            var state = TestSetup.StateAt(TestSetup.Single, 0);

            var result = TestSetup.CreateExecutor().Execute(state, new SkipTutorialSequenceCommand(TestSetup.Single));

            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(TestSetup.Single, state.Tutorial.ActiveSequenceId);
        }

        [Test]
        public void Skip_FlagOff_IsRejected()
        {
            var state = TestSetup.StateAt(TestSetup.NoSkip, 0);

            var result = TestSetup.CreateExecutor().Execute(state, new SkipTutorialSequenceCommand(TestSetup.NoSkip));

            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(TestSetup.NoSkip, state.Tutorial.ActiveSequenceId);
        }

        [Test]
        public void Skip_SequenceNotActive_IsRejected()
        {
            var state = TestSetup.StateAt(TestSetup.Intro, 0);

            var result = TestSetup.CreateExecutor().Execute(state, new SkipTutorialSequenceCommand(TestSetup.NoSkip));

            Assert.IsFalse(result.IsSuccess);
            Assert.AreEqual(TestSetup.Intro, state.Tutorial.ActiveSequenceId);
        }
    }
}
