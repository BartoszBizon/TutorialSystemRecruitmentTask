using System;
using System.Collections.Generic;

namespace ArtifexMundi.Shared.Commands
{
    public sealed class CommandResult
    {
        private CommandResult(bool isSuccess, string error, IReadOnlyList<IChange> changes)
        {
            IsSuccess = isSuccess;
            Error = error;
            Changes = changes;
        }

        public bool IsSuccess { get; }
        public string Error { get; }
        public IReadOnlyList<IChange> Changes { get; }

        public static CommandResult Success(IReadOnlyList<IChange> changes) => new CommandResult(true, null, changes);
        public static CommandResult Failure(string error) => new CommandResult(false, error, Array.Empty<IChange>());
    }
}
