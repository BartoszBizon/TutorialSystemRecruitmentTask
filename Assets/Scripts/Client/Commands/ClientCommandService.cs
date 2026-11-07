using System;
using System.Collections.Generic;
using ArtifexMundi.Client.Model;
using ArtifexMundi.Client.Network;
using ArtifexMundi.Shared.Commands;
using ArtifexMundi.Shared.Network;
using ArtifexMundi.Shared.State;
using UnityEngine;

namespace ArtifexMundi.Client.Commands
{
    public interface ICommandSender
    {
        /// <summary>Executes the command locally (prediction) and sends it to the server.</summary>
        CommandResult Send(ICommand command);
    }

    /// <summary>
    /// Optimistic execution (client-side prediction):
    /// 1. Execute in local Shared Logic -> UI reacts immediately, no waiting for the server.
    /// 2. Send the same command to the server.
    /// 3. Server rejects -> take its authoritative state, re-apply still pending commands (reconciliation).
    /// </summary>
    public sealed class ClientCommandService : ICommandSender, IGameStateProvider, ICommandResultNotifier
    {
        private readonly CommandExecutor _executor;
        private readonly IServerConnection _connection;
        private readonly List<CommandRequest> _pending = new List<CommandRequest>();
        private int _nextRequestId;

        public ClientCommandService(CommandExecutor executor, IServerConnection connection, GameState initialState)
        {
            _executor = executor ?? throw new ArgumentNullException(nameof(executor));
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
            State = initialState ?? throw new ArgumentNullException(nameof(initialState));
        }

        /// <summary>Client copy of the state. Instance is replaced on resync - always read it through this property.</summary>
        public GameState State { get; private set; }

        /// <summary>"Change (command result)" arrow - consumed by the Model layer.</summary>
        public event Action<IReadOnlyList<IChange>> ChangesApplied;

        /// <summary>State was replaced by the server's one - Model has to refresh everything.</summary>
        public event Action StateResynced;

        public CommandResult Send(ICommand command)
        {
            var result = _executor.Execute(State, command);
            if (!result.IsSuccess)
            {
                // Rejected locally - the server would reject it too, no need to send.
                Debug.LogWarning($"[Client] {command.GetType().Name} rejected: {result.Error}");
                return result;
            }

            var request = new CommandRequest(++_nextRequestId, command);
            _pending.Add(request);
            _connection.Send(request, OnResponse);

            ChangesApplied?.Invoke(result.Changes);
            return result;
        }

        private void OnResponse(CommandResponse response)
        {
            _pending.RemoveAll(r => r.RequestId == response.RequestId);

            if (response.IsSuccess)
                return;

            Debug.LogWarning($"[Client] Server rejected request {response.RequestId}: {response.Error}. Resyncing.");
            if (response.AuthoritativeState == null)
                return;

            State = response.AuthoritativeState;
            foreach (var pending in _pending)
                _executor.Execute(State, pending.Command);

            StateResynced?.Invoke();
        }
    }
}
