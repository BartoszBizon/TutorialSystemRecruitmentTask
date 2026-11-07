using System;
using System.Collections.Generic;
using ArtifexMundi.Client.Network;
using ArtifexMundi.Server;
using ArtifexMundi.Shared.Network;
using ArtifexMundi.Shared.State;

namespace ArtifexMundi.Demo.Network
{
    /// <summary>
    /// In-process "network": the server handles a request immediately, the response is delivered
    /// after a simulated latency. Shows that the UI does not wait for the server (prediction).
    /// Must be ticked from the main thread.
    /// </summary>
    public sealed class LocalServerConnection : IServerConnection
    {
        private readonly GameServer _server;
        private readonly float _latencySeconds;
        private readonly Queue<(float deliverAt, Action deliver)> _inFlight = new Queue<(float, Action)>();
        private float _time;
        private string _playerId;

        public LocalServerConnection(GameServer server, float latencySeconds)
        {
            _server = server;
            _latencySeconds = latencySeconds;
        }

        public void Login(string playerId, Action<GameState> onLoggedIn)
        {
            _playerId = playerId;
            var state = _server.Login(playerId);
            Enqueue(() => onLoggedIn(state));
        }

        public void Send(CommandRequest request, Action<CommandResponse> onResponse)
        {
            var response = _server.Handle(_playerId, request);
            Enqueue(() => onResponse(response));
        }

        public void Tick(float deltaTime)
        {
            _time += deltaTime;
            while (_inFlight.Count > 0 && _inFlight.Peek().deliverAt <= _time)
                _inFlight.Dequeue().deliver();
        }

        private void Enqueue(Action deliver)
        {
            _inFlight.Enqueue((_time + _latencySeconds, deliver));
        }
    }
}
