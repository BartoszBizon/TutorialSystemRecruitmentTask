using ArtifexMundi.Shared.Commands;
using ArtifexMundi.Shared.Config;
using ArtifexMundi.Shared.Tutorial.Processors;

namespace ArtifexMundi.Shared
{
    /// <summary>
    /// Builds the command pipeline. Client and server MUST use the same pipeline,
    /// otherwise client prediction and server authority would produce different results.
    /// </summary>
    public static class SharedLogicFactory
    {
        public static CommandExecutor CreateExecutor(GameConfig config)
        {
            return new CommandExecutor(
                config,
                new ICommandPreProcessor[] { new TutorialBlockingPreProcessor() },
                new ICommandPostProcessor[] { new TutorialProgressionPostProcessor() });
        }
    }
}
