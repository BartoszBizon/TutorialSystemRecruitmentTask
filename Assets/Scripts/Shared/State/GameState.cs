using System;
using ArtifexMundi.Shared.Game;
using ArtifexMundi.Shared.Tutorial;

namespace ArtifexMundi.Shared.State
{
    /// <summary>
    /// Whole player state, owned by the server and mirrored on the client.
    /// Plain serializable data - no logic besides cloning.
    /// </summary>
    [Serializable]
    public sealed class GameState
    {
        public TutorialState Tutorial = new TutorialState();
        public PlayerState Player = new PlayerState();

        public GameState()
        {
        }

        public GameState(TutorialState tutorial, PlayerState player)
        {
            Tutorial = tutorial;
            Player = player;
        }

        public GameState Clone()
        {
            var newTutorial = Tutorial.Clone();
            var newPlayer = Player.Clone();
            GameState state = new GameState(newTutorial, newPlayer);
            return state;
        }
    }
}
