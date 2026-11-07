using ArtifexMundi.Shared.State;

namespace ArtifexMundi.Client.Model
{
    /// <summary>Read access to the client's copy of the state (instance may be replaced on resync).</summary>
    public interface IGameStateProvider
    {
        GameState State { get; }
    }
}
