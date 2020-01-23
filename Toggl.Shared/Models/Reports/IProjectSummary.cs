namespace Toggl.Shared.Models.Reports
{
    public interface IProjectSummary
    {
        long? ProjectId { get; }
        long TrackedSeconds { get; }
        long? BillableSeconds { get; }
    }
}
