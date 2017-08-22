﻿﻿using System;
using System.Collections.Generic;
using Toggl.PrimeRadiant.Models;
using Toggl.Multivac;

namespace Toggl.Foundation.Models
{
    internal partial class Client : IDatabaseClient
    {
        public long Id { get; }

        public long WorkspaceId { get; }

        public string Name { get; }

        public DateTimeOffset At { get; }

        public DateTimeOffset? ServerDeletedAt { get; }

        public IDatabaseWorkspace Workspace { get; }

        public bool IsDirty { get; }
    }

    internal partial class Project : IDatabaseProject
    {
        public long Id { get; }

        public long WorkspaceId { get; }

        public long? ClientId { get; }

        public string Name { get; }

        public bool IsPrivate { get; }

        public bool Active { get; }

        public DateTimeOffset At { get; }

        public DateTimeOffset? ServerDeletedAt { get; }

        public string Color { get; }

        public bool? Billable { get; }

        public bool? Template { get; }

        public bool? AutoEstimates { get; }

        public int? EstimatedHours { get; }

        public double? Rate { get; }

        public string Currency { get; }

        public int? ActualHours { get; }

        public IDatabaseClient Client { get; }

        public IDatabaseWorkspace Workspace { get; }

        public bool IsDirty { get; }
    }

    internal partial class Tag : IDatabaseTag
    {
        public long Id { get; }

        public long WorkspaceId { get; }

        public string Name { get; }

        public DateTimeOffset At { get; }

        public IDatabaseWorkspace Workspace { get; }

        public bool IsDirty { get; }
    }

    internal partial class Task : IDatabaseTask
    {
        public long Id { get; }

        public string Name { get; }

        public long ProjectId { get; }

        public long WorkspaceId { get; }

        public long? UserId { get; }

        public long EstimatedSeconds { get; }

        public bool Active { get; }

        public DateTimeOffset At { get; }

        public long TrackedSeconds { get; }

        public IDatabaseUser User { get; }

        public IDatabaseProject Project { get; }

        public IDatabaseWorkspace Workspace { get; }

        public bool IsDirty { get; }
    }

    internal partial class TimeEntry : IDatabaseTimeEntry
    {
        public long Id { get; }

        public long WorkspaceId { get; }

        public long? ProjectId { get; }

        public long? TaskId { get; }

        public bool Billable { get; }

        public DateTimeOffset Start { get; }

        public DateTimeOffset? Stop { get; }

        public string Description { get; }

        public IList<string> TagNames { get; }

        public IList<long> TagIds { get; }

        public DateTimeOffset At { get; }

        public DateTimeOffset? ServerDeletedAt { get; }

        public long UserId { get; }

        public string CreatedWith { get; }

        public bool IsDeleted { get; }

        public IDatabaseTask Task { get; }

        public IDatabaseUser User { get; }

        public IDatabaseProject Project { get; }

        public IDatabaseWorkspace Workspace { get; }

        public bool IsDirty { get; }
    }

    internal partial class User : IDatabaseUser
    {
        public long Id { get; }

        public string ApiToken { get; }

        public long DefaultWorkspaceId { get; }

        public string Email { get; }

        public string Fullname { get; }

        public string TimeOfDayFormat { get; }

        public string DateFormat { get; }

        public bool StoreStartAndStopTime { get; }

        public BeginningOfWeek BeginningOfWeek { get; }

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
        public long Id { get; }

        public string Name { get; }

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