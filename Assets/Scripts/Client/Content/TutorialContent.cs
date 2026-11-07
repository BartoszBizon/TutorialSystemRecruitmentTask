using System;
using System.Collections.Generic;
using System.Linq;
using ArtifexMundi.Shared.Game;
using ArtifexMundi.Shared.Tutorial;
using ArtifexMundi.Shared.Tutorial.Triggers;
using UnityEngine;

namespace ArtifexMundi.Client.Content
{
    public enum DialogSide
    {
        Left,
        Right
    }

    public enum HighlightStyle
    {
        /// <summary>Hole in a darkened screen.</summary>
        Cutout,
        /// <summary>Additive effect on top of the object - screen stays undimmed (used with dialogs).</summary>
        Additive
    }

    [Serializable]
    public sealed class TutorialDialogContent
    {
        public bool Enabled;
        public string Speaker;
        [TextArea] public string Text;
        public DialogSide Side;
    }

    [Serializable]
    public sealed class TutorialIndicationContent
    {
        public bool Enabled;
        /// <summary>Id of a TutorialAnchor in the scene.</summary>
        public string TargetId;
        public HighlightStyle Highlight;
        public bool ShowPointer = true;
        /// <summary>Optional screen to open when a blocking target is not on screen (e.g. after restart).</summary>
        public string ScreenId;
    }

    public enum TutorialTriggerType
    {
        SequenceCompleted,
        GoldAtLeast
    }

    [Serializable]
    public sealed class TutorialTriggerContent
    {
        public TutorialTriggerType Type;
        public string SequenceId;
        public int Amount;

        public ITutorialTrigger ToTrigger()
        {
            switch (Type)
            {
                case TutorialTriggerType.SequenceCompleted: return new SequenceCompletedTrigger(SequenceId);
                case TutorialTriggerType.GoldAtLeast: return new GoldAtLeastTrigger(Amount);
                default: throw new ArgumentOutOfRangeException(nameof(Type), Type, null);
            }
        }
    }

    /// <summary>
    /// Designer-authored entry: logic part (goes to Shared Logic / server) + presentation part (client only).
    /// </summary>
    [Serializable]
    public sealed class TutorialEntryContent
    {
        public string Id;
        public bool IsBlocking;
        [Tooltip("Empty = completed from UI (dialog V / UI-only tap).")]
        public string CompletionActionKey;
        public TutorialDialogContent Dialog = new TutorialDialogContent();
        public TutorialIndicationContent Indication = new TutorialIndicationContent();

        public bool IsDialogOnly => Dialog.Enabled && !Indication.Enabled;

        public TutorialEntryDefinition ToDefinition()
        {
            var actionKey = string.IsNullOrEmpty(CompletionActionKey) ? null : CompletionActionKey;
            return new TutorialEntryDefinition(Id, IsBlocking, actionKey);
        }
    }

    [Serializable]
    public sealed class TutorialSequenceContent
    {
        public string Id;
        public bool IsSkippable;
        [Tooltip("All must be satisfied. Empty = start immediately.")]
        public List<TutorialTriggerContent> StartConditions = new List<TutorialTriggerContent>();
        public List<TutorialEntryContent> Entries = new List<TutorialEntryContent>();

        public TutorialSequenceDefinition ToDefinition()
        {
            ITutorialTrigger trigger = StartConditions.Count == 0
                ? AlwaysTrigger.Instance
                : new AllOfTrigger(StartConditions.Select(c => c.ToTrigger()).ToArray());

            return new TutorialSequenceDefinition(
                Id,
                Entries.Select(e => e.ToDefinition()).ToList(),
                IsSkippable,
                trigger);
        }
    }
}
