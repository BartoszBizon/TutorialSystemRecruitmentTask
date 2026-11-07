using ArtifexMundi.Client.Binding;
using ArtifexMundi.Client.ViewModel;
using UnityEngine;
using UnityEngine.UI;

namespace ArtifexMundi.Client.View
{
    /// <summary>
    /// Root of the tutorial overlay (own canvas, sorted above game UI). Binds TutorialViewModel to sub-views.
    /// Sibling order inside: blocker -> highlight -> dialog / skip, so "V" and Skip stay clickable while blocked.
    /// </summary>
    public sealed class TutorialView : MonoBehaviour
    {
        [SerializeField] private TutorialDialogView _dialog;
        [SerializeField] private TutorialHighlightView _highlight;
        [SerializeField] private TutorialInputBlocker _blocker;
        [SerializeField] private Button _skipButton;

        private readonly BindingSet _bindings = new BindingSet();
        private TutorialViewModel _viewModel;
        private TutorialTargetRegistry _targets;

        // Indication follows its target every frame (targets can move / animate / scroll).
        private void LateUpdate()
        {
            if (_viewModel == null)
                return;

            if (_viewModel.IsIndicationVisible && _targets.TryGetScreenRect(_viewModel.IndicationTargetId, out var rect))
            {
                _highlight.Show(rect, _viewModel.HighlightStyle, _viewModel.ShowPointer);
                _blocker.SetHole(rect);
            }
            else
            {
                _highlight.Hide();
                _blocker.SetHole(null);
            }
        }

        private void OnDestroy()
        {
            _bindings.Dispose();
        }

        public void Bind(TutorialViewModel viewModel, TutorialTargetRegistry targets)
        {
            _viewModel = viewModel;
            _targets = targets;

            _bindings
                .Bind(viewModel, nameof(viewModel.IsDialogVisible), () => viewModel.IsDialogVisible, _dialog.SetVisible)
                .Bind(viewModel, nameof(viewModel.DialogSpeaker), () => viewModel.DialogSpeaker, _dialog.SetSpeaker)
                .Bind(viewModel, nameof(viewModel.DialogText), () => viewModel.DialogText, _dialog.SetText)
                .Bind(viewModel, nameof(viewModel.DialogSide), () => viewModel.DialogSide, _dialog.SetSide)
                .Bind(viewModel, nameof(viewModel.DialogDots), () => viewModel.DialogDots, _dialog.SetDots)
                .Bind(viewModel, nameof(viewModel.IsContinueVisible), () => viewModel.IsContinueVisible, _dialog.SetContinueVisible)
                .Bind(viewModel, nameof(viewModel.IsInputBlocked), () => viewModel.IsInputBlocked, blocked => _blocker.gameObject.SetActive(blocked))
                .Bind(viewModel, nameof(viewModel.IsSkipVisible), () => viewModel.IsSkipVisible, visible => _skipButton.gameObject.SetActive(visible))
                .BindCommand(_dialog.ContinueButton, viewModel.ContinueCommand)
                .BindCommand(_skipButton, viewModel.SkipCommand)
                .BindEvent<string>(h => targets.TargetTapped += h, h => targets.TargetTapped -= h, viewModel.TargetTappedCommand);
        }
    }
}
