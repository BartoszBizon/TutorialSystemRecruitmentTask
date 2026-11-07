using ArtifexMundi.Shared.Commands;

namespace ArtifexMundi.Shared.Network
{
    /// <summary>
    /// "Command request" arrow from the client/server diagram.
    /// A real transport would serialize Command with a type discriminator (JSON / MessagePack).
    /// </summary>
    public sealed class CommandRequest
    {
        public CommandRequest(int requestId, ICommand command)
        {
            RequestId = requestId;
            Command = command;
        }

        public int RequestId { get; }
        public ICommand Command { get; }
    }
}
