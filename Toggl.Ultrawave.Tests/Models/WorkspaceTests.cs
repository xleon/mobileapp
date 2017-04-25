namespace Toggl.Ultrawave.Tests.Models
{
    public class WorkspaceTests
    {
        public class TheWorkspaceModel : BaseModelTests<Workspace>
        {
            protected override string ValidJson => 
                "{\"id\":1234,\"name\":\"Default workspace\",\"profile\":0,\"premium\":false,\"business_ws\":false,\"admin\":true,\"SuspendedAt\":\"2018-04-24T12:16:48+00:00\",\"server_deleted_at\":\"2017-04-20T12:16:48+00:00\",\"default_hourly_rate\":0,\"default_currency\":\"USD\",\"only_admins_may_create_projects\":false,\"only_admins_see_billable_rates\":false,\"only_admins_see_team_dashboard\":false,\"projects_billable_by_default\":true,\"rounding\":0,\"rounding_minutes\":0,\"at\":\"2017-04-24T12:16:48+00:00\",\"logo_url\":\"https://assets.toggl.com/images/workspace.jpg\"}";
            
            protected override Workspace ValidObject => new Workspace
            {
                Id = 1234,
                Name = "Default workspace",
                Profile = 0,
                Premium = false,
                BusinessWs = false,
                Admin = true,
                SuspendedAt = "2018-04-24T12:16:48+00:00",
                ServerDeletedAt = "2017-04-20T12:16:48+00:00",
                DefaultHourlyRate = 0,
                DefaultCurrency = "USD",
                OnlyAdminsMayCreateProjects = false,
                OnlyAdminsSeeBillableRates = false,
                OnlyAdminsSeeTeamDashboard = false,
                ProjectsBillableByDefault = true,
                Rounding = 0,
                RoundingMinutes = 0,
                At = "2017-04-24T12:16:48+00:00",
                LogoUrl = "https://assets.toggl.com/images/workspace.jpg"
            };
        }
    }
}