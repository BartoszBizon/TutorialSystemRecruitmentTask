using System;

namespace ArtifexMundi.Client.ViewModel
{
    /// <summary>
    /// Which tutorial targets (TutorialAnchors) are currently on screen. Implemented by the View layer;
    /// ViewModel only needs to know availability, never positions or GameObjects.
    /// </summary>
    public interface ITutorialTargetRegistry
    {
        event Action TargetsChanged;
        bool IsAvailable(string targetId);
    }

    /// <summary>Opens a screen by id - used to bring a blocking tutorial target on screen after restart.</summary>
    public interface IScreenNavigator
    {
        void Show(string screenId);
    }
}
