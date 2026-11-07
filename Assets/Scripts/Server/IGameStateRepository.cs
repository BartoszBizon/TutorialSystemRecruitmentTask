using ArtifexMundi.Shared.State;

namespace ArtifexMundi.Server
{
    /// <summary>
    /// Persistent storage of player state (database on a real server).
    /// </summary>
    public interface IGameStateRepository
    {
        bool TryLoad(string playerId, out GameState state);
        void Save(string playerId, GameState state);
    }
}
