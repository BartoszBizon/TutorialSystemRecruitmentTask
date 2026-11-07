using ArtifexMundi.Server;
using ArtifexMundi.Shared.State;
using UnityEngine;

namespace ArtifexMundi.Demo.Persistence
{
    /// <summary>
    /// Demo stand-in for the server database - survives killing the game, so restoring
    /// the exact tutorial entry after restart can be verified.
    /// </summary>
    public sealed class PlayerPrefsGameStateRepository : IGameStateRepository
    {
        private const string KeyPrefix = "demo_save_";

        public bool TryLoad(string playerId, out GameState state)
        {
            var json = PlayerPrefs.GetString(KeyPrefix + playerId, null);
            state = string.IsNullOrEmpty(json) ? null : JsonUtility.FromJson<GameState>(json);
            return state != null;
        }

        public void Save(string playerId, GameState state)
        {
            PlayerPrefs.SetString(KeyPrefix + playerId, JsonUtility.ToJson(state));
            PlayerPrefs.Save();
        }

        public static void Delete(string playerId)
        {
            PlayerPrefs.DeleteKey(KeyPrefix + playerId);
        }
    }
}
