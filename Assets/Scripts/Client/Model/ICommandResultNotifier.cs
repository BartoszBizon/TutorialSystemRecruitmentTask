using System;
using System.Collections.Generic;
using ArtifexMundi.Shared.Commands;

namespace ArtifexMundi.Client.Model
{
    /// <summary>
    /// Source of "Change (command result)" for the Model layer. Owned by the Model (consumer),
    /// implemented by ClientCommandService - Model does not depend on the concrete command/network code.
    /// </summary>
    public interface ICommandResultNotifier
    {
        event Action<IReadOnlyList<IChange>> ChangesApplied;

        /// <summary>Whole state was replaced by the server's one.</summary>
        event Action StateResynced;
    }
}
