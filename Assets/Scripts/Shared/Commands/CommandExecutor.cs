using System;
using System.Collections.Generic;
using ArtifexMundi.Shared.Config;
using ArtifexMundi.Shared.State;

namespace ArtifexMundi.Shared.Commands
{
    /// <summary>
    /// Entry point of Shared Logic. The same instance type runs on client and server.
    /// </summary>
    public sealed class CommandExecutor
    {
        private readonly GameConfig _config;
        private readonly IReadOnlyList<ICommandPreProcessor> _preProcessors;
        private readonly IReadOnlyList<ICommandPostProcessor> _postProcessors;

        public CommandExecutor(
            GameConfig config,
            IReadOnlyList<ICommandPreProcessor> preProcessors = null,
            IReadOnlyList<ICommandPostProcessor> postProcessors = null)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            _preProcessors = preProcessors ?? Array.Empty<ICommandPreProcessor>();
            _postProcessors = postProcessors ?? Array.Empty<ICommandPostProcessor>();
        }

        public CommandResult Execute(GameState state, ICommand command)
        {
            if (state == null) throw new ArgumentNullException(nameof(state));
            if (command == null) throw new ArgumentNullException(nameof(command));

            var context = new CommandContext(state, _config);

            foreach (var preProcessor in _preProcessors)
            {
                var error = preProcessor.Validate(context, command);
                if (error != null)
                    return CommandResult.Failure(error);
            }

            var validationError = command.Validate(context);
            if (validationError != null)
                return CommandResult.Failure(validationError);

            command.Execute(context);

            foreach (var postProcessor in _postProcessors)
                postProcessor.OnExecuted(context, command);

            return CommandResult.Success(context.Changes);
        }
    }
}
