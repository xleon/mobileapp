﻿using System;
using Toggl.PrimeRadiant.Models;

namespace Toggl.Foundation.Models
{
    internal class FoundationClient : IDatabaseClient
    {
        public int Id { get; set; }

        public int WorkspaceId { get; set; }

        public string Name { get; set; }

        public DateTimeOffset At { get; set; }

        public bool IsDirty { get; set; }
    }

    internal class FoundationProject : IDatabaseProject
    {
        public int Id { get; set; }

        public bool IsDirty { get; set; }
    }

    internal class FoundationTag : IDatabaseTag
    {
        public int Id { get; set; }

        public int WorkspaceId { get; set; }

        public string Name { get; set; }

        public bool IsDirty { get; set; }
    }

    internal class FoundationTask : IDatabaseTask
    {
        public int Id { get; set; }

        public bool IsDirty { get; set; }
    }

    internal class FoundationTimeEntry : IDatabaseTimeEntry
    {
        public int Id { get; set; }

        public bool IsDirty { get; set; }
    }

    internal class FoundationUser : IDatabaseUser
    {
        public int Id { get; set; }

        public string ApiToken { get; set; }

        public int DefaultWorkspaceId { get; set; }

        public string Email { get; set; }

        public string Fullname { get; set; }

        public string TimeOfDayFormat { get; set; }

        public string DateFormat { get; set; }

        public bool StoreStartAndStopTime { get; set; }

        public int BeginningOfWeek { get; set; }

        public string Language { get; set; }

        public string ImageUrl { get; set; }

        public bool SidebarPiechart { get; set; }

        public DateTimeOffset At { get; set; }

        public int Retention { get; set; }

        public bool RecordTimeline { get; set; }

        public bool RenderTimeline { get; set; }

        public bool TimelineEnabled { get; set; }

        public bool TimelineExperiment { get; set; }

        public bool IsDirty { get; set; }
    }

    internal class FoundationWorkspace : IDatabaseWorkspace
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int Profile { get; set; }

        public bool Premium { get; set; }

        public bool BusinessWs { get; set; }

        public bool Admin { get; set; }

        public DateTimeOffset? SuspendedAt { get; set; }

        public DateTimeOffset? ServerDeletedAt { get; set; }

        public double? DefaultHourlyRate { get; set; }

        public string DefaultCurrency { get; set; }

        public bool OnlyAdminsMayCreateProjects { get; set; }

        public bool OnlyAdminsSeeBillableRates { get; set; }

        public bool OnlyAdminsSeeTeamDashboard { get; set; }

        public bool ProjectsBillableByDefault { get; set; }

        public int Rounding { get; set; }

        public int RoundingMinutes { get; set; }

        public DateTimeOffset? At { get; set; }

        public string LogoUrl { get; set; }

        public bool IsDirty { get; set; }
    }
}