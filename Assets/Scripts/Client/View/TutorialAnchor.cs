using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ArtifexMundi.Client.View
{
    /// <summary>
    /// Marks a UI element or world object as a tutorial target. Content references it by TargetId,
    /// so tutorial data never holds scene references. Registered only while enabled -
    /// "target is on screen" == "anchor is enabled".
    /// </summary>
    public sealed class TutorialAnchor : MonoBehaviour, IPointerClickHandler
    {
        // Components can't get constructor injection, so enabled anchors are tracked statically
        // (same idea as Camera.allCameras). TutorialTargetRegistry adapts this to ITutorialTargetRegistry.
        private static readonly Dictionary<string, TutorialAnchor> Enabled = new Dictionary<string, TutorialAnchor>();

        // Shared buffer - GetScreenRect is called every frame while an indication is visible, avoid GC allocations.
        private static readonly Vector3[] Corners = new Vector3[4];

        [SerializeField] private string _targetId;

        private RectTransform _rectTransform;
        private Canvas _canvas;
        private Renderer _renderer;

        internal static event Action AnchorsChanged;
        internal static event Action<string> AnchorTapped;

        private void Awake()
        {
            _rectTransform = transform as RectTransform;
            _renderer = _rectTransform == null ? GetComponentInChildren<Renderer>() : null;
            CacheCanvas();
        }

        private void OnEnable()
        {
            if (string.IsNullOrEmpty(_targetId))
                return;

            if (Enabled.TryGetValue(_targetId, out var other) && other != this)
                Debug.LogWarning($"[Tutorial] Two enabled anchors with id '{_targetId}': {other.name}, {name}.", this);

            Enabled[_targetId] = this;
            AnchorsChanged?.Invoke();
        }

        private void OnDisable()
        {
            if (string.IsNullOrEmpty(_targetId) || !Enabled.TryGetValue(_targetId, out var current) || current != this)
                return;

            Enabled.Remove(_targetId);
            AnchorsChanged?.Invoke();
        }

        // Re-parenting may move the anchor to another canvas.
        private void OnTransformParentChanged() => CacheCanvas();

        public void OnPointerClick(PointerEventData eventData)
        {
            AnchorTapped?.Invoke(_targetId);
        }

        internal static bool TryGet(string targetId, out TutorialAnchor anchor)
        {
            anchor = null;
            return !string.IsNullOrEmpty(targetId) && Enabled.TryGetValue(targetId, out anchor);
        }

        /// <summary>Target bounds in screen pixels.</summary>
        public Rect GetScreenRect()
        {
            if (_rectTransform != null)
            {
                var camera = _canvas != null && _canvas.renderMode != RenderMode.ScreenSpaceOverlay ? _canvas.worldCamera : null;
                _rectTransform.GetWorldCorners(Corners);
                Vector2 min = RectTransformUtility.WorldToScreenPoint(camera, Corners[0]);
                Vector2 max = RectTransformUtility.WorldToScreenPoint(camera, Corners[2]);
                return Rect.MinMaxRect(min.x, min.y, max.x, max.y);
            }

            // World object - project renderer bounds.
            var mainCamera = Camera.main;
            if (mainCamera == null)
                return default;
            if (_renderer == null)
                return new Rect(mainCamera.WorldToScreenPoint(transform.position), Vector2.zero);

            var bounds = _renderer.bounds;
            Vector2 a = mainCamera.WorldToScreenPoint(bounds.min);
            Vector2 b = mainCamera.WorldToScreenPoint(bounds.max);
            return Rect.MinMaxRect(Mathf.Min(a.x, b.x), Mathf.Min(a.y, b.y), Mathf.Max(a.x, b.x), Mathf.Max(a.y, b.y));
        }

        private void CacheCanvas()
        {
            _canvas = _rectTransform != null ? GetComponentInParent<Canvas>() : null;
        }
    }
}
