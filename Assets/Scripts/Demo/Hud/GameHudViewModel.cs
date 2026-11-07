using System;
using ArtifexMundi.Client.Commands;
using ArtifexMundi.Client.Model;
using ArtifexMundi.Client.Mvvm;
using ArtifexMundi.Client.ViewModel;
using ArtifexMundi.Demo.Content;
using ArtifexMundi.Shared.Game;

namespace ArtifexMundi.Demo.Hud
{
    /// <summary>
    /// Demo gameplay UI. Knows nothing about the tutorial - it just sends gameplay commands;
    /// the tutorial advances in Shared Logic when the expected action happens.
    /// </summary>
    public sealed class GameHudViewModel : ViewModelBase, IDisposable
    {
        private readonly PlayerModel _player;
        private readonly ICommandSender _commandSender;
        private readonly IDisposable _subscription;

        private int _gold;
        private int _strengthLevel;

        public GameHudViewModel(PlayerModel player, ICommandSender commandSender, IScreenNavigator navigator, IMessenger messenger)
        {
            _player = player;
            _commandSender = commandSender;

            CollectRewardCommand = new RelayCommand(() => _commandSender.Send(new CollectRewardCommand()));
            UpgradeStrengthCommand = new RelayCommand(
                () => _commandSender.Send(new UpgradeSkillCommand(DemoIds.StrengthSkill)),
                () => Gold >= _player.Economy.SkillUpgradeCost);
            OpenSkillsCommand = new RelayCommand(() => navigator.Show(DemoIds.SkillsScreen));
            CloseSkillsCommand = new RelayCommand(() => navigator.Show(DemoIds.MainScreen));

            _subscription = messenger.Subscribe<PlayerChangedMessage>(_ => Refresh());
            Refresh();
        }

        public int Gold { get => _gold; private set => SetProperty(ref _gold, value); }
        public int StrengthLevel { get => _strengthLevel; private set => SetProperty(ref _strengthLevel, value); }
        public int UpgradeCost => _player.Economy.SkillUpgradeCost;

        public RelayCommand CollectRewardCommand { get; }
        public RelayCommand UpgradeStrengthCommand { get; }
        public RelayCommand OpenSkillsCommand { get; }
        public RelayCommand CloseSkillsCommand { get; }

        public void Dispose() => _subscription.Dispose();

        private void Refresh()
        {
            Gold = _player.Gold;
            StrengthLevel = _player.GetSkillLevel(DemoIds.StrengthSkill);
            UpgradeStrengthCommand.RaiseCanExecuteChanged();
        }
    }
}
