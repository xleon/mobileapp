using Toggl.Multivac;
using Toggl.Multivac.Models;

namespace Toggl.Ultrawave.Models
{
    public sealed partial class WorkspaceFeature : IWorkspaceFeature
    {
        public string Name { get; set; }

        public WorkspaceFeatureId FeatureId { get; set; }

        public bool Enabled { get; set; }
    }
}
