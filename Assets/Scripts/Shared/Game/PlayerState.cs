using System;
using System.Collections.Generic;

namespace ArtifexMundi.Shared.Game
{
    [Serializable]
    public sealed class SkillLevel
    {
        public string SkillId;
        public int Level;
    }

    /// <summary>
    /// Minimal demo gameplay state - gives the tutorial real actions to teach (collect reward, upgrade skill).
    /// </summary>
    [Serializable]
    public sealed class PlayerState
    {
        public int Gold;
        public List<SkillLevel> Skills = new List<SkillLevel>();

        public PlayerState()
        {
        }

        public PlayerState(int gold, List<SkillLevel> skills)
        {
            Gold = gold;
            Skills = new List<SkillLevel>();
            foreach (var skill in skills ?? new List<SkillLevel>())
                Skills.Add(new SkillLevel { SkillId = skill.SkillId, Level = skill.Level });
        }

        public int GetSkillLevel(string skillId)
        {
            var skill = Skills.Find(s => s.SkillId == skillId);
            return skill?.Level ?? 0;
        }

        public void SetSkillLevel(string skillId, int level)
        {
            var skill = Skills.Find(s => s.SkillId == skillId);
            if (skill == null)
                Skills.Add(new SkillLevel { SkillId = skillId, Level = level });
            else
                skill.Level = level;
        }

        public PlayerState Clone()
        {
            return new PlayerState(Gold, Skills);
        }
    }
}
