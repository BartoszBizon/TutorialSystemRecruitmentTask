using ArtifexMundi.Client.Binding;
using UnityEngine;
using UnityEngine.UI;

namespace ArtifexMundi.Demo.Hud
{
    public sealed class GameHudView : MonoBehaviour
    {
        [SerializeField] private Text _gold;
        [SerializeField] private Text _strength;
        [SerializeField] private Text _upgradeLabel;
        [SerializeField] private Button _rewardButton;
        [SerializeField] private Button _skillsButton;
        [SerializeField] private Button _upgradeButton;
        [SerializeField] private Button _closeSkillsButton;

        private readonly BindingSet _bindings = new BindingSet();

        private void OnDestroy() => _bindings.Dispose();

        public void Bind(GameHudViewModel viewModel)
        {
            _upgradeLabel.text = $"Upgrade Strength\n({viewModel.UpgradeCost} gold)";

            _bindings
                .Bind(viewModel, nameof(viewModel.Gold), () => viewModel.Gold, gold => _gold.text = $"Gold: {gold}")
                .Bind(viewModel, nameof(viewModel.StrengthLevel), () => viewModel.StrengthLevel, level => _strength.text = $"Strength: {level}")
                .BindCommand(_rewardButton, viewModel.CollectRewardCommand)
                .BindCommand(_skillsButton, viewModel.OpenSkillsCommand)
                .BindCommand(_upgradeButton, viewModel.UpgradeStrengthCommand)
                .BindCommand(_closeSkillsButton, viewModel.CloseSkillsCommand);
        }
    }
}
