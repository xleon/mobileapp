using Realms;
using Toggl.Multivac.Models;
using Toggl.PrimeRadiant.Models;

namespace Toggl.PrimeRadiant.Realm
{
    internal partial class RealmClient
    {
        [PrimaryKey]
        public int Id { get; set; }

        public bool IsDirty { get; set; }

        public RealmClient() { }

        public RealmClient(IDatabaseClient entity)
            : this(entity as IClient)
        {
            IsDirty = entity.IsDirty;
        }

        public RealmClient(IClient entity)
        {
            Id = entity.Id;
            WorkspaceId = entity.WorkspaceId;
            Name = entity.Name;
            At = entity.At;
            IsDirty = true;
        }
    }

    internal partial class RealmProject
    {
        [PrimaryKey]
        public int Id { get; set; }

        public bool IsDirty { get; set; }

        public RealmProject() { }

        public RealmProject(IDatabaseProject entity)
            : this(entity as IProject)
        {
            IsDirty = entity.IsDirty;
        }

        public RealmProject(IProject entity)
        {
            Id = entity.Id;
            IsDirty = true;
        }
    }

    internal partial class RealmTag
    {
        [PrimaryKey]
        public int Id { get; set; }

        public bool IsDirty { get; set; }

        public RealmTag() { }

        public RealmTag(IDatabaseTag entity)
            : this(entity as ITag)
        {
            IsDirty = entity.IsDirty;
        }

        public RealmTag(ITag entity)
        {
            Id = entity.Id;
            WorkspaceId = entity.WorkspaceId;
            Name = entity.Name;
            IsDirty = true;
        }
    }

    internal partial class RealmTask
    {
        [PrimaryKey]
        public int Id { get; set; }

        public bool IsDirty { get; set; }

        public RealmTask() { }

        public RealmTask(IDatabaseTask entity)
            : this(entity as ITask)
        {
            IsDirty = entity.IsDirty;
        }

        public RealmTask(ITask entity)
        {
            Id = entity.Id;
            IsDirty = true;
        }
    }

    internal partial class RealmTimeEntry
    {
        [PrimaryKey]
        public int Id { get; set; }

        public bool IsDirty { get; set; }

        public RealmTimeEntry() { }

        public RealmTimeEntry(IDatabaseTimeEntry entity)
            : this(entity as ITimeEntry)
        {
            IsDirty = entity.IsDirty;
        }

        public RealmTimeEntry(ITimeEntry entity)
        {
            Id = entity.Id;
            WorkspaceId = entity.WorkspaceId;
            ProjectId = entity.ProjectId;
            TaskId = entity.TaskId;
            Billable = entity.Billable;
            Start = entity.Start;
            Stop = entity.Stop;
            Duration = entity.Duration;
            Description = entity.Description;
            Tags = entity.Tags;
            TagIds = entity.TagIds;
            At = entity.At;
            ServerDeletedAt = entity.ServerDeletedAt;
            UserId = entity.UserId;
            CreatedWith = entity.CreatedWith;
            IsDirty = true;
        }
    }

    internal partial class RealmUser
    {
        [PrimaryKey]
        public int Id { get; set; }

        public bool IsDirty { get; set; }

        public RealmUser() { }

        public RealmUser(IDatabaseUser entity)
            : this(entity as IUser)
        {
            IsDirty = entity.IsDirty;
        }

        public RealmUser(IUser entity)
        {
            Id = entity.Id;
            ApiToken = entity.ApiToken;
            DefaultWorkspaceId = entity.DefaultWorkspaceId;
            Email = entity.Email;
            Fullname = entity.Fullname;
            TimeOfDayFormat = entity.TimeOfDayFormat;
            DateFormat = entity.DateFormat;
            StoreStartAndStopTime = entity.StoreStartAndStopTime;
            BeginningOfWeek = entity.BeginningOfWeek;
            Language = entity.Language;
            ImageUrl = entity.ImageUrl;
            SidebarPiechart = entity.SidebarPiechart;
            At = entity.At;
            Retention = entity.Retention;
            RecordTimeline = entity.RecordTimeline;
            RenderTimeline = entity.RenderTimeline;
            TimelineEnabled = entity.TimelineEnabled;
            TimelineExperiment = entity.TimelineExperiment;
            IsDirty = true;
        }
    }

    internal partial class RealmWorkspace
    {
        [PrimaryKey]
        public int Id { get; set; }

        public bool IsDirty { get; set; }

        public RealmWorkspace() { }

        public RealmWorkspace(IDatabaseWorkspace entity)
            : this(entity as IWorkspace)
        {
            IsDirty = entity.IsDirty;
        }

        public RealmWorkspace(IWorkspace entity)
        {
            Id = entity.Id;
            Name = entity.Name;
            Profile = entity.Profile;
            Premium = entity.Premium;
            BusinessWs = entity.BusinessWs;
            Admin = entity.Admin;
            SuspendedAt = entity.SuspendedAt;
            ServerDeletedAt = entity.ServerDeletedAt;
            DefaultHourlyRate = entity.DefaultHourlyRate;
            DefaultCurrency = entity.DefaultCurrency;
            OnlyAdminsMayCreateProjects = entity.OnlyAdminsMayCreateProjects;
            OnlyAdminsSeeBillableRates = entity.OnlyAdminsSeeBillableRates;
            OnlyAdminsSeeTeamDashboard = entity.OnlyAdminsSeeTeamDashboard;
            ProjectsBillableByDefault = entity.ProjectsBillableByDefault;
            Rounding = entity.Rounding;
            RoundingMinutes = entity.RoundingMinutes;
            At = entity.At;
            LogoUrl = entity.LogoUrl;
            IsDirty = true;
        }
    }
}