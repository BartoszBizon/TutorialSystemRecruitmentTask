using ArtifexMundi.Shared.Commands;

namespace ArtifexMundi.Shared.Game
{
    public sealed class GoldChanged : IChange
    {
        public GoldChanged(int gold) => Gold = gold;

        public int Gold { get; }
    }

    public sealed class SkillLevelChanged : IChange
    {
        public SkillLevelChanged(string skillId, int level)
        {
            SkillId = skillId;
            Level = level;
        }

        public string SkillId { get; }
        public int Level { get; }
    }
}
