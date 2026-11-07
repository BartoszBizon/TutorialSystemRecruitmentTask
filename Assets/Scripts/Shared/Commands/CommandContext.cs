using System.Collections.Generic;
using ArtifexMundi.Shared.Config;
using ArtifexMundi.Shared.State;

namespace ArtifexMundi.Shared.Commands
{
    public sealed class CommandContext
    {
        private readonly List<IChange> _changes = new List<IChange>();

        public CommandContext(GameState state, GameConfig config)
        {
            State = state;
            Config = config;
        }

        public GameState State { get; }
        public GameConfig Config { get; }
        public IReadOnlyList<IChange> Changes => _changes;

        public void Emit(IChange change)
        {
            _changes.Add(change);
        }
    }
}
