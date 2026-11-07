using System;
using System.Collections.Generic;
using ArtifexMundi.Shared.Commands;

namespace ArtifexMundi.Client.Model
{
    /// <summary>
    /// Model layer root: receives "Change (command result)" from Shared Logic and dispatches it to models.
    /// </summary>
    public sealed class ClientModel : IDisposable
    {
        private readonly IReadOnlyList<IModel> _models;
        private readonly Action<IReadOnlyList<IChange>> _changesHandler;
        private readonly Action _resyncHandler;
        private readonly ICommandResultNotifier _commandResults;

        public ClientModel(ICommandResultNotifier commandResults, PlayerModel player, TutorialModel tutorial)
        {
            _commandResults = commandResults;
            Player = player;
            Tutorial = tutorial;
            _models = new IModel[] { player, tutorial };

            _changesHandler = OnChangesApplied;
            _resyncHandler = Refresh;
            _commandResults.ChangesApplied += _changesHandler;
            _commandResults.StateResynced += _resyncHandler;
        }

        public PlayerModel Player { get; }
        public TutorialModel Tutorial { get; }

        public void Refresh()
        {
            foreach (var model in _models)
                model.Refresh();
        }

        public void Dispose()
        {
            _commandResults.ChangesApplied -= _changesHandler;
            _commandResults.StateResynced -= _resyncHandler;
        }

        private void OnChangesApplied(IReadOnlyList<IChange> changes)
        {
            foreach (var change in changes)
            foreach (var model in _models)
                model.Apply(change);
        }
    }
}
