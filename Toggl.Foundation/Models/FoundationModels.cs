using System;
using System.Collections.Generic;
using Toggl.PrimeRadiant.Models;

namespace Toggl.Foundation.Models
{
    internal partial class Client : IDatabaseClient
    {
        public int Id { get; }

        public int WorkspaceId { get; }

        public string Name { get; }

        public DateTimeOffset At { get; }

        public bool IsDirty { get; }
    }

    internal partial class Project : IDatabaseProject
    {
        public int Id { get; }

        public bool IsDirty { get; }
    }

    internal partial class Tag : IDatabaseTag
    {
        public int Id { get; }

        public int WorkspaceId { get; }

        public string Name { get; }

        public bool IsDirty { get; }
    }

    internal partial class Task : IDatabaseTask
    {
        public int Id { get; }

        public bool IsDirty { get; }
    }

    internal partial class TimeEntry : IDatabaseTimeEntry
    {
        public int Id { get; }

        public int WorkspaceId { get; }

        public int? ProjectId { get; }

        public int? TaskId { get; }

        public bool Billable { get; }

        public DateTimeOffset Start { get; }

        public DateTimeOffset? Stop { get; }

        public int Duration { get; }

        public string Description { get; }

        public IList<string> Tags { get; }

        public IList<int> TagIds { get; }

        public DateTimeOffset At { get; }

        public DateTimeOffset? ServerDeletedAt { get; }

        public int UserId { get; }

        public string CreatedWith { get; }

        public bool IsDirty { get; }
    }

    internal partial class User : IDatabaseUser
    {
        public int Id { get; }

        public string ApiToken { get; }

        public int DefaultWorkspaceId { get; }

        public string Email { get; }

        public string Fullname { get; }

        public string TimeOfDayFormat { get; }

        public string DateFormat { get; }

        public bool StoreStartAndStopTime { get; }

        public int BeginningOfWeek { get; }

        public string Language { get; }

        public string ImageUrl { get; }

        public bool SidebarPiechart { get; }

        public DateTimeOffset At { get; }

        public int Retention { get; }

        public bool RecordTimeline { get; }

        public bool RenderTimeline { get; }

        public bool TimelineEnabled { get; }

        public bool TimelineExperiment { get; }

        public bool IsDirty { get; }
    }

    internal partial class Workspace : IDatabaseWorkspace
    {
        public int Id { get; }

        public string Name { get; }

        public int Profile { get; }

        public bool Premium { get; }

        public bool BusinessWs { get; }

        public bool Admin { get; }

        public DateTimeOffset? SuspendedAt { get; }

        public DateTimeOffset? ServerDeletedAt { get; }

        public double? DefaultHourlyRate { get; }

        public string DefaultCurrency { get; }

        public bool OnlyAdminsMayCreateProjects { get; }

        public bool OnlyAdminsSeeBillableRates { get; }

        public bool OnlyAdminsSeeTeamDashboard { get; }

        public bool ProjectsBillableByDefault { get; }

        public int Rounding { get; }

        public int RoundingMinutes { get; }

        public DateTimeOffset? At { get; }

        public string LogoUrl { get; }

        public bool IsDirty { get; }
    }
}