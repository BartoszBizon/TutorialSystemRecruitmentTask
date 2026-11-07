using System;
using ArtifexMundi.Shared.Network;
using ArtifexMundi.Shared.State;

namespace ArtifexMundi.Client.Network
{
    /// <summary>
    /// Client's gateway to the server. The client does not know whether the server is local (demo) or remote.
    /// Callbacks are invoked on the main thread.
    /// </summary>
    public interface IServerConnection
    {
        void Login(string playerId, Action<GameState> onLoggedIn);
        void Send(CommandRequest request, Action<CommandResponse> onResponse);
    }
}
