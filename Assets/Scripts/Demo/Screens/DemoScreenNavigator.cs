using ArtifexMundi.Client.ViewModel;
using ArtifexMundi.Demo.Content;
using UnityEngine;

namespace ArtifexMundi.Demo.Screens
{
    /// <summary>Two demo screens. Switching disables the other screen - and with it its TutorialAnchors.</summary>
    public sealed class DemoScreenNavigator : IScreenNavigator
    {
        private readonly GameObject _mainScreen;
        private readonly GameObject _skillsScreen;

        public DemoScreenNavigator(GameObject mainScreen, GameObject skillsScreen)
        {
            _mainScreen = mainScreen;
            _skillsScreen = skillsScreen;
        }

        public void Show(string screenId)
        {
            var skills = screenId == DemoIds.SkillsScreen;
            _mainScreen.SetActive(!skills);
            _skillsScreen.SetActive(skills);
        }
    }
}
