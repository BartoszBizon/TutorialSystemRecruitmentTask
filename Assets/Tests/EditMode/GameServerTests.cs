using System.Collections.Generic;
using ArtifexMundi.Server;
using ArtifexMundi.Shared.Network;
using ArtifexMundi.Shared.Session;
using ArtifexMundi.Shared.State;
using ArtifexMundi.Shared.Tutorial.Commands;
using NUnit.Framework;
using UnityEngine;

namespace ArtifexMundi.Tests
{
    /// <summary>Server authority and "kill & restart returns to the exact entry".</summary>
    public sealed class GameServerTests
    {
        private const string PlayerId = "player";

        [Test]
        public void Restart_AfterProgress_RestoresExactEntry()
        {
            var repository = new JsonRepository();
            var server = CreateServer(repository);
            server.Login(PlayerId);
            server.Handle(PlayerId, new CommandRequest(1, new StartSessionCommand()));
            server.Handle(PlayerId, new CommandRequest(2, new CompleteTutorialEntryCommand(TestSetup.Intro, 0)));

            // "Kill" the game: new server instance, same storage.
            var restored = CreateServer(repository).Login(PlayerId);

            Assert.AreEqual(TestSetup.Intro, restored.Tutorial.ActiveSequenceId);
            Assert.AreEqual(1, restored.Tutorial.ActiveEntryIndex);
        }

        [Test]
        public void ForbiddenCommand_IsRejectedWithAuthoritativeState()
        {
            var server = CreateServer(new JsonRepository());
            server.Login(PlayerId);
            server.Handle(PlayerId, new CommandRequest(1, new StartSessionCommand()));

            // Modified client tries to complete an entry that requires collecting the reward.
            var response = server.Handle(PlayerId, new CommandRequest(2, new CompleteTutorialEntryCommand(TestSetup.Intro, 1)));

            Assert.IsFalse(response.IsSuccess);
            Assert.AreEqual(TestSetup.Intro, response.AuthoritativeState.Tutorial.ActiveSequenceId);
            Assert.AreEqual(0, response.AuthoritativeState.Tutorial.ActiveEntryIndex);
        }

        [Test]
        public void Login_ReturnsCopy_ClientChangesDoNotAffectServer()
        {
            var server = CreateServer(new JsonRepository());
            var clientState = server.Login(PlayerId);

            clientState.Tutorial.ActiveSequenceId = TestSetup.NoSkip;
            var response = server.Handle(PlayerId, new CommandRequest(1, new SkipTutorialSequenceCommand(TestSetup.NoSkip)));

            Assert.IsFalse(response.IsSuccess);
            Assert.IsNull(response.AuthoritativeState.Tutorial.ActiveSequenceId);
        }

        private static GameServer CreateServer(IGameStateRepository repository) =>
            new GameServer(TestSetup.CreateExecutor(), repository, () => new GameState());

        /// <summary>Stores JSON like the demo repository - also verifies the state survives serialization.</summary>
        private sealed class JsonRepository : IGameStateRepository
        {
            private readonly Dictionary<string, string> _json = new Dictionary<string, string>();

            public bool TryLoad(string playerId, out GameState state)
            {
                state = _json.TryGetValue(playerId, out var json) ? JsonUtility.FromJson<GameState>(json) : null;
                return state != null;
            }

            public void Save(string playerId, GameState state) => _json[playerId] = JsonUtility.ToJson(state);
        }
    }
}
