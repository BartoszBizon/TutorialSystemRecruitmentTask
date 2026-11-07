using ArtifexMundi.Client.Mvvm;
using ArtifexMundi.Shared.Commands;
using ArtifexMundi.Shared.Game;

namespace ArtifexMundi.Client.Model
{
    public sealed class PlayerModel : IModel
    {
        private readonly IGameStateProvider _stateProvider;
        private readonly IMessenger _messenger;

        public PlayerModel(IGameStateProvider stateProvider, EconomyConfig economy, IMessenger messenger)
        {
            _stateProvider = stateProvider;
            Economy = economy;
            _messenger = messenger;
        }

        public EconomyConfig Economy { get; }

        public int Gold => _stateProvider.State.Player.Gold;

        public int GetSkillLevel(string skillId) => _stateProvider.State.Player.GetSkillLevel(skillId);

        public void Apply(IChange change)
        {
            if (change is GoldChanged || change is SkillLevelChanged)
                _messenger.Publish(new PlayerChangedMessage());
        }

        public void Refresh()
        {
            _messenger.Publish(new PlayerChangedMessage());
        }
    }
}
