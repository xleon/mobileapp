using System.Linq;
using Toggl.Multivac;
using Toggl.Multivac.Models;

namespace Toggl.Foundation.Models
{
    internal partial class WorkspaceFeatureCollection
    {
        public bool IsEnabled(WorkspaceFeatureId feature)
            => Features.Any(x => x.FeatureId == feature);

        public static WorkspaceFeatureCollection From(IWorkspaceFeatureCollection entity)
            => new WorkspaceFeatureCollection(entity);
    }
}
