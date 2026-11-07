using ArtifexMundi.Client.Content;
using UnityEngine;
using UnityEngine.UI;

namespace ArtifexMundi.Client.View
{
    /// <summary>
    /// Tutorial dialog box: no background dimming (scene stays interactive), docked left or right
    /// and narrow enough to keep the screen center free.
    /// </summary>
    public sealed class TutorialDialogView : MonoBehaviour
    {
        [SerializeField] private RectTransform _panel;
        [SerializeField] private Text _speaker;
        [SerializeField] private Text _body;
        [SerializeField] private Text _continueLabel;
        [SerializeField] private Button _continueButton;
        [SerializeField, Range(0.2f, 0.45f)] private float _widthFraction = 0.35f;
        [SerializeField] private float _margin = 24f;

        public Button ContinueButton => _continueButton;

        public void SetVisible(bool visible) => _panel.gameObject.SetActive(visible);
        public void SetSpeaker(string speaker) => _speaker.text = speaker;
        public void SetText(string text) => _body.text = text;
        public void SetContinueVisible(bool visible) => _continueButton.gameObject.SetActive(visible);

        /// <summary>"•• V" - one dot per following dialog-only entry.</summary>
        public void SetDots(int dots) => _continueLabel.text = new string('•', dots) + (dots > 0 ? " " : string.Empty) + "V";

        public void SetSide(DialogSide side)
        {
            var left = side == DialogSide.Left;
            _panel.anchorMin = new Vector2(left ? 0f : 1f - _widthFraction, 0f);
            _panel.anchorMax = new Vector2(left ? _widthFraction : 1f, 0f);
            _panel.pivot = new Vector2(left ? 0f : 1f, 0f);
            _panel.anchoredPosition = new Vector2(left ? _margin : -_margin, _margin);
            _panel.sizeDelta = new Vector2(0f, _panel.sizeDelta.y);
        }
    }
}
