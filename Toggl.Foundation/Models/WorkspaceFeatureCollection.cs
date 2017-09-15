using System.Linq;
using Toggl.Multivac;

namespace Toggl.Foundation.Models
{
    internal partial class WorkspaceFeatureCollection
    {
        public bool IsEnabled(WorkspaceFeatureId feature)
            => Features.Any(x => x.FeatureId == feature);
    }
}
