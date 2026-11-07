using System;
using System.Collections.Generic;
using ArtifexMundi.Client.Commands;
using ArtifexMundi.Client.Content;
using ArtifexMundi.Client.Model;
using ArtifexMundi.Client.Mvvm;
using ArtifexMundi.Client.View;
using ArtifexMundi.Client.ViewModel;
using ArtifexMundi.Demo.Content;
using ArtifexMundi.Demo.Hud;
using ArtifexMundi.Demo.Network;
using ArtifexMundi.Demo.Persistence;
using ArtifexMundi.Demo.Screens;
using ArtifexMundi.Server;
using ArtifexMundi.Shared;
using ArtifexMundi.Shared.Config;
using ArtifexMundi.Shared.Game;
using ArtifexMundi.Shared.Session;
using ArtifexMundi.Shared.State;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace ArtifexMundi.Demo
{
    /// <summary>
    /// Composition Root - the only place that creates and wires everything (manual dependency injection).
    /// Server and client get separate executors and separate state copies, exactly as over a real network.
    /// </summary>
    public sealed class TutorialDemoBootstrap : MonoBehaviour
    {
        private const string PlayerId = "demo_player";

        [SerializeField] private TutorialView _tutorialView;
        [Tooltip("Optional - delayed hint shown after inactivity.")]
        [SerializeField] private IdleHintView _idleHintView;
        [SerializeField] private float _idleHintTimeoutSeconds = 5f;
        [SerializeField] private GameHudView _hudView;
        [SerializeField] private GameObject _mainScreen;
        [SerializeField] private GameObject _skillsScreen;
        [SerializeField] private Button _resetProgressButton;
        [Tooltip("Designer-authored tutorial sequences (Create > ArtifexMundi > Tutorial Content).")]
        [SerializeField] private TutorialContentAsset _content;
        [SerializeField] private int _rewardGold = 100;
        [SerializeField] private int _skillUpgradeCost = 100;
        [SerializeField] private float _simulatedLatencySeconds = 3f;

        private readonly List<IDisposable> _disposables = new List<IDisposable>();
        private LocalServerConnection _connection;

        private void Start()
        {
            _resetProgressButton.onClick.AddListener(ResetProgress);

            if (_content == null)
            {
                Debug.LogError("[Tutorial] Assign Tutorial Content asset (Assets/Content/TutorialContent.asset) to TutorialDemoBootstrap.", this);
                enabled = false;
                return;
            }

            var catalog = _content.CreateCatalog();
            var config = new GameConfig(catalog.BuildConfig(), new EconomyConfig(_rewardGold, _skillUpgradeCost));

            // --- Server side ---
            var server = new GameServer(
                SharedLogicFactory.CreateExecutor(config),
                new PlayerPrefsGameStateRepository(),
                () => new GameState());
            _connection = new LocalServerConnection(server, _simulatedLatencySeconds);

            // --- Client side (after login) ---
            _connection.Login(PlayerId, state => OnLoggedIn(state, config, catalog));
        }

        private void Update()
        {
            _connection?.Tick(Time.unscaledDeltaTime);
        }

        private void OnDestroy()
        {
            foreach (var disposable in _disposables)
                disposable.Dispose();
            _disposables.Clear();
        }

        private void OnLoggedIn(GameState state, GameConfig config, TutorialContentCatalog catalog)
        {
            var commandService = new ClientCommandService(SharedLogicFactory.CreateExecutor(config), _connection, state);
            var messenger = new Messenger();

            // Model layer
            var model = new ClientModel(
                commandService,
                new PlayerModel(commandService, config.Economy, messenger),
                new TutorialModel(commandService, config.Tutorial, messenger));
            _disposables.Add(model);

            // ViewModel layer
            var navigator = new DemoScreenNavigator(_mainScreen, _skillsScreen);
            navigator.Show(DemoIds.MainScreen);
            var targets = new TutorialTargetRegistry();
            _disposables.Add(targets);

            var tutorialViewModel = new TutorialViewModel(model.Tutorial, catalog, commandService, targets, navigator, messenger);
            var hudViewModel = new GameHudViewModel(model.Player, commandService, navigator, messenger);
            _disposables.Add(tutorialViewModel);
            _disposables.Add(hudViewModel);

            // Binding + View layer
            _tutorialView.Bind(tutorialViewModel, targets);
            _hudView.Bind(hudViewModel);

            if (_idleHintView != null)
            {
                var idleHintViewModel = new IdleHintViewModel(
                    model.Tutorial,
                    new DemoIdleHintTargetProvider(model.Player),
                    targets,
                    messenger,
                    _idleHintTimeoutSeconds);
                _disposables.Add(idleHintViewModel);
                _idleHintView.Bind(idleHintViewModel, targets);
            }

            // Starts the first tutorial sequence whose trigger is satisfied (or resumes the saved one).
            commandService.Send(new StartSessionCommand());
        }

        private void ResetProgress()
        {
            PlayerPrefsGameStateRepository.Delete(PlayerId);
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}
