using UnityEngine;
using UnityEngine.UI;

namespace ArtifexMundi.Client.View
{
    /// <summary>
    /// Full-screen invisible raycast target for blocking entries. Catches every tap except inside the hole,
    /// so the tap falls through to the indicated object underneath.
    /// Tutorial canvas must be sorted above the game UI.
    /// </summary>
    [RequireComponent(typeof(Image))]
    public sealed class TutorialInputBlocker : MonoBehaviour, ICanvasRaycastFilter
    {
        private Rect? _hole;

        public void SetHole(Rect? screenRect) => _hole = screenRect;

        public bool IsRaycastLocationValid(Vector2 screenPoint, Camera eventCamera)
        {
            return !(_hole.HasValue && _hole.Value.Contains(screenPoint));
        }
    }
}
