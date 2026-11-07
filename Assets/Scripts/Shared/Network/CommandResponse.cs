using ArtifexMundi.Shared.State;

namespace ArtifexMundi.Shared.Network
{
    /// <summary>
    /// "Command result response" arrow from the client/server diagram.
    /// On rejection the server sends its authoritative state so the client can reconcile.
    /// </summary>
    public sealed class CommandResponse
    {
        private CommandResponse(int requestId, bool isSuccess, string error, GameState authoritativeState)
        {
            RequestId = requestId;
            IsSuccess = isSuccess;
            Error = error;
            AuthoritativeState = authoritativeState;
        }

        public int RequestId { get; }
        public bool IsSuccess { get; }
        public string Error { get; }

        /// <summary>Only set when the command was rejected.</summary>
        public GameState AuthoritativeState { get; }

        public static CommandResponse Accepted(int requestId) => new CommandResponse(requestId, true, null, null);

        public static CommandResponse Rejected(int requestId, string error, GameState authoritativeState) =>
            new CommandResponse(requestId, false, error, authoritativeState);
    }
}
