using System.Collections.Generic;
using Toggl.Networking.Models.Reports;
using Xunit;

namespace Toggl.Networking.Tests.Models.Reports
{
    public sealed class ProjectsSummaryTests
    {
        public sealed class TheProjectsSummaryModel
        {
            private string validJson
                => "[{\"project_id\":null,\"tracked_seconds\":9876,\"billable_seconds\":6543},"
                    + "{\"project_id\":1427273,\"tracked_seconds\":5678,\"billable_seconds\":null},"
                    + "{\"project_id\":1427273,\"tracked_seconds\":4598,\"billable_seconds\":56}]";

            private List<ProjectSummary> validSummary => new List<ProjectSummary>
            {
                new ProjectSummary
                {
                    ProjectId = null,
                    TrackedSeconds = 9876,
                    BillableSeconds = 6543
                },
                new ProjectSummary
                {
                    ProjectId = 1427273,
                    TrackedSeconds = 5678,
                    BillableSeconds = null
                },
                new ProjectSummary
                {
                    ProjectId = 1427273,
                    TrackedSeconds = 4598,
                    BillableSeconds = 56
                }
            };

            [Fact, LogIfTooSlow]
            public void CanBeDeserialized()
            {
                SerializationHelper.CanBeDeserialized(validJson, validSummary);
            }

            [Fact, LogIfTooSlow]
            public void CanBeSerialized()
            {
                SerializationHelper.CanBeSerialized(validJson, validSummary);
            }
        }
    }
}
