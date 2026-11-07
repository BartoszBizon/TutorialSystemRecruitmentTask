using ArtifexMundi.Client.Content;
using UnityEngine;

namespace ArtifexMundi.Client.View
{
    /// <summary>
    /// Indication visuals (placeholders - final art from artists):
    /// - Cutout: four dimming panels around the target = "hole" in a darkened screen,
    /// - Additive: pulsing frame on top of the target, screen not dimmed,
    /// - Pointer: bobbing hand/arrow next to the target.
    /// Visual only - never catches raycasts (blocking is TutorialInputBlocker's job).
    /// </summary>
    public sealed class TutorialHighlightView : MonoBehaviour
    {
        [SerializeField] private RectTransform _root;
        [SerializeField] private RectTransform _dimTop;
        [SerializeField] private RectTransform _dimBottom;
        [SerializeField] private RectTransform _dimLeft;
        [SerializeField] private RectTransform _dimRight;
        [SerializeField] private RectTransform _frame;
        [SerializeField] private RectTransform _pointer;
        [SerializeField] private float _padding = 12f;
        [SerializeField] private float _pulseSpeed = 4f;
        [SerializeField] private float _pointerBob = 18f;

        private HighlightStyle _style;
        private bool _showPointer;
        private Canvas _canvas;

        private void Awake()
        {
            // Prefab can't reference the canvas it will be placed in - fall back to the root canvas.
            if (_root == null)
            {
                var parentCanvas = GetComponentInParent<Canvas>();
                if (parentCanvas == null)
                {
                    Debug.LogError("[Tutorial] TutorialHighlightView must be placed under a Canvas.", this);
                    return;
                }

                _root = (RectTransform)parentCanvas.rootCanvas.transform;
            }

            _canvas = _root.GetComponentInParent<Canvas>();
        }

        public void Show(Rect screenRect, HighlightStyle style, bool showPointer)
        {
            _style = style;
            _showPointer = showPointer;
            gameObject.SetActive(true);
            Layout(screenRect);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        private void Layout(Rect screenRect)
        {
            var camera = _canvas != null && _canvas.renderMode != RenderMode.ScreenSpaceOverlay ? _canvas.worldCamera : null;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(_root, screenRect.min, camera, out var min);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(_root, screenRect.max, camera, out var max);
            min -= Vector2.one * _padding;
            max += Vector2.one * _padding;

            var bounds = _root.rect;
            var cutout = _style == HighlightStyle.Cutout;
            _dimTop.gameObject.SetActive(cutout);
            _dimBottom.gameObject.SetActive(cutout);
            _dimLeft.gameObject.SetActive(cutout);
            _dimRight.gameObject.SetActive(cutout);
            if (cutout)
            {
                Place(_dimTop, new Vector2(bounds.xMin, max.y), new Vector2(bounds.xMax, bounds.yMax));
                Place(_dimBottom, new Vector2(bounds.xMin, bounds.yMin), new Vector2(bounds.xMax, min.y));
                Place(_dimLeft, new Vector2(bounds.xMin, min.y), new Vector2(min.x, max.y));
                Place(_dimRight, new Vector2(max.x, min.y), new Vector2(bounds.xMax, max.y));
            }

            var additive = _style == HighlightStyle.Additive;
            _frame.gameObject.SetActive(additive);
            if (additive)
            {
                Place(_frame, min, max);
                _frame.localScale = Vector3.one * (1f + 0.06f * Mathf.Sin(Time.unscaledTime * _pulseSpeed));
            }

            _pointer.gameObject.SetActive(_showPointer);
            if (_showPointer)
            {
                var bob = Mathf.Abs(Mathf.Sin(Time.unscaledTime * _pulseSpeed * 0.75f)) * _pointerBob;
                _pointer.anchoredPosition = new Vector2(max.x, min.y) + new Vector2(bob, -bob) * 0.7f;
            }
        }

        private static void Place(RectTransform rect, Vector2 min, Vector2 max)
        {
            rect.anchorMin = rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = new Vector2(Mathf.Max(0f, max.x - min.x), Mathf.Max(0f, max.y - min.y));
            rect.anchoredPosition = (min + max) * 0.5f;
        }
    }
}
