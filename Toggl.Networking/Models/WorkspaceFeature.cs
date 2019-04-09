using Toggl.Shared;
using Toggl.Shared.Models;

namespace Toggl.Ultrawave.Models
{
    internal sealed partial class WorkspaceFeature : IWorkspaceFeature
    {
        public string Name { get; set; }

        public WorkspaceFeatureId FeatureId { get; set; }

        public bool Enabled { get; set; }
    }
}
