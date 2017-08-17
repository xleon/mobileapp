namespace Toggl.Multivac.Models
{
    public interface IWorkspaceFeature
    {
        WorkspaceFeatureId FeatureId { get; }

        bool Enabled { get; }
    }
}
