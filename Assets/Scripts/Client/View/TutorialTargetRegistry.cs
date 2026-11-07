using System;
using ArtifexMundi.Client.ViewModel;
using UnityEngine;

namespace ArtifexMundi.Client.View
{
    /// <summary>
    /// Adapter: exposes enabled TutorialAnchors to the ViewModel (availability only)
    /// and to Views (screen rects, taps).
    /// </summary>
    public sealed class TutorialTargetRegistry : ITutorialTargetRegistry, IDisposable
    {
        public TutorialTargetRegistry()
        {
            TutorialAnchor.AnchorsChanged += RaiseTargetsChanged;
            TutorialAnchor.AnchorTapped += RaiseTargetTapped;
        }

        public event Action TargetsChanged;
        public event Action<string> TargetTapped;

        public bool IsAvailable(string targetId) => TutorialAnchor.TryGet(targetId, out _);

        public bool TryGetScreenRect(string targetId, out Rect rect)
        {
            if (TutorialAnchor.TryGet(targetId, out var anchor))
            {
                rect = anchor.GetScreenRect();
                return true;
            }

            rect = default;
            return false;
        }

        public void Dispose()
        {
            TutorialAnchor.AnchorsChanged -= RaiseTargetsChanged;
            TutorialAnchor.AnchorTapped -= RaiseTargetTapped;
        }

        private void RaiseTargetsChanged() => TargetsChanged?.Invoke();
        private void RaiseTargetTapped(string targetId) => TargetTapped?.Invoke(targetId);
    }
}
