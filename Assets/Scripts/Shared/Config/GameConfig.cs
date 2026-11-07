using System;
using ArtifexMundi.Shared.Game;
using ArtifexMundi.Shared.Tutorial;

namespace ArtifexMundi.Shared.Config
{
    /// <summary>
    /// Static game data (designer-authored), identical on client and server. Available to every command.
    /// </summary>
    public sealed class GameConfig
    {
        public GameConfig(TutorialConfig tutorial, EconomyConfig economy)
        {
            Tutorial = tutorial ?? throw new ArgumentNullException(nameof(tutorial));
            Economy = economy ?? throw new ArgumentNullException(nameof(economy));
        }

        public TutorialConfig Tutorial { get; }
        public EconomyConfig Economy { get; }
    }
}
