using System;
using ArtifexMundi.Client.Content;
using ArtifexMundi.Client.ViewModel;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

namespace ArtifexMundi.Client.View
{
    /// <summary>
    /// Delayed hint: reuses TutorialHighlightView (same visuals as a standard indication),
    /// never blocks input. Reports any button/touch/mouse press as an interaction.
    /// Must live on an always-active object (not on the highlight itself - Hide() deactivates it).
    /// </summary>
    public sealed class IdleHintView : MonoBehaviour
    {
        [SerializeField] private TutorialHighlightView _highlight;
        [SerializeField] private HighlightStyle _style = HighlightStyle.Additive;
        [SerializeField] private bool _showPointer = true;

        private IDisposable _anyInput;
        private IdleHintViewModel _viewModel;
        private TutorialTargetRegistry _targets;

        private void Update()
        {
            if (_viewModel == null)
                return;

            _viewModel.Tick(Time.unscaledDeltaTime);
        }

        private void LateUpdate()
        {
            if (_viewModel == null)
                return;

            if (_viewModel.HintTargetId != null && _targets.TryGetScreenRect(_viewModel.HintTargetId, out var rect))
                _highlight.Show(rect, _style, _showPointer);
            else
                _highlight.Hide();
        }

        private void OnDestroy()
        {
            _anyInput?.Dispose();
        }

        public void Bind(IdleHintViewModel viewModel, TutorialTargetRegistry targets)
        {
            _viewModel = viewModel;
            _targets = targets;
            _anyInput = InputSystem.onAnyButtonPress.Call(_ => viewModel.InteractionCommand.Execute());
            _highlight.Hide();
        }
    }
}
