namespace Toggl.Multivac.Models
{
    public interface IWorkspace : IBaseModel
    {
        string Name { get; }

        int Profile { get; }

        bool Premium { get; }

        bool BusinessWs { get; }

        bool Admin { get; }

        string SuspendedAt { get; }

        string ServerDeletedAt { get; }

        double DefaultHourlyRate { get; }

        string DefaultCurrency { get; }

        bool OnlyAdminsMayCreateProjects { get; }

        bool OnlyAdminsSeeBillableRates { get; }

        bool OnlyAdminsSeeTeamDashboard { get; }

        bool ProjectsBillableByDefault { get; }

        int Rounding { get; }

        int RoundingMinutes { get; }

        string At { get; }

        string LogoUrl { get; }
    }
}
