using ArtifexMundi.Shared.Commands;

namespace ArtifexMundi.Client.Model
{
    public interface IModel
    {
        /// <summary>Single change from a command result. Ignore changes that don't belong to this model.</summary>
        void Apply(IChange change);

        /// <summary>Whole state was replaced (login / server resync) - notify everything.</summary>
        void Refresh();
    }
}
