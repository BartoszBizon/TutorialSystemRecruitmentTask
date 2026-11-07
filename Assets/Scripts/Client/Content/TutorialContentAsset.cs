using System.Collections.Generic;
using UnityEngine;

namespace ArtifexMundi.Client.Content
{
    /// <summary>
    /// Designer-facing asset with all tutorial sequences (order = start priority).
    /// </summary>
    [CreateAssetMenu(fileName = "TutorialContent", menuName = "ArtifexMundi/Tutorial Content")]
    public sealed class TutorialContentAsset : ScriptableObject
    {
        [SerializeField] private List<TutorialSequenceContent> _sequences = new List<TutorialSequenceContent>();

        public TutorialContentCatalog CreateCatalog() => new TutorialContentCatalog(_sequences);

        // Shows content mistakes to the designer right after editing, not at runtime.
        private void OnValidate()
        {
            foreach (var error in TutorialContentValidator.Validate(_sequences))
                Debug.LogError($"[Tutorial] {name}: {error}", this);
        }
    }
}
