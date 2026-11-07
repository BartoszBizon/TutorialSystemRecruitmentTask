using System;
using System.Collections.Generic;
using ArtifexMundi.Shared.Commands;
using ArtifexMundi.Shared.Network;
using ArtifexMundi.Shared.State;

namespace ArtifexMundi.Server
{
    /// <summary>
    /// Authoritative side: runs the same Shared Logic as the client and persists the result.
    /// </summary>
    public sealed class GameServer
    {
        private readonly CommandExecutor _executor;
        private readonly IGameStateRepository _repository;
        private readonly Func<GameState> _newPlayerStateFactory;
        private readonly Dictionary<string, GameState> _sessions = new Dictionary<string, GameState>();

        public GameServer(CommandExecutor executor, IGameStateRepository repository, Func<GameState> newPlayerStateFactory)
        {
            _executor = executor ?? throw new ArgumentNullException(nameof(executor));
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _newPlayerStateFactory = newPlayerStateFactory ?? throw new ArgumentNullException(nameof(newPlayerStateFactory));
        }

        /// <summary>Returns a copy of the player's state - client and server never share instances.</summary>
        public GameState Login(string playerId)
        {
            if (!_repository.TryLoad(playerId, out var state))
            {
                state = _newPlayerStateFactory();
                _repository.Save(playerId, state);
            }

            _sessions[playerId] = state;
            return state.Clone();
        }

        public CommandResponse Handle(string playerId, CommandRequest request)
        {
            if (!_sessions.TryGetValue(playerId, out var state))
                return CommandResponse.Rejected(request.RequestId, "Player is not logged in.", null);

            var result = _executor.Execute(state, request.Command);
            if (!result.IsSuccess)
                return CommandResponse.Rejected(request.RequestId, result.Error, state.Clone());

            // Persist after every command - killing the game restores the exact tutorial entry.
            _repository.Save(playerId, state);
            return CommandResponse.Accepted(request.RequestId);
        }
    }
}
