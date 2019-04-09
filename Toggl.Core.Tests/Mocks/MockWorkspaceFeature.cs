using Toggl.Core.Models.Interfaces;
using Toggl.Multivac;

namespace Toggl.Core.Tests.Mocks
{
    public sealed class MockWorkspaceFeature : IThreadSafeWorkspaceFeature
    {
        public WorkspaceFeatureId FeatureId { get; set; }

        public bool Enabled { get; set; }
    }
}
