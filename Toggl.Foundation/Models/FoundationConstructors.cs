﻿using Toggl.Multivac.Models;
using Toggl.PrimeRadiant;

namespace Toggl.Foundation.Models
{
    internal partial class Client
    {
        private Client(IClient entity, SyncStatus syncStatus)
        {
            Id = entity.Id;
            WorkspaceId = entity.WorkspaceId;
            Name = entity.Name;
            At = entity.At;
            ServerDeletedAt = entity.ServerDeletedAt;
            SyncStatus = syncStatus;
        }

        public static Client Clean(IClient entity)
            => new Client(entity, SyncStatus.InSync);

        public static Client Dirty(IClient entity)
            => new Client(entity, SyncStatus.SyncNeeded);

        public static Client Unsyncable(IClient entity)
            => new Client(entity, SyncStatus.SyncFailed);
    }

    internal partial class Project
    {
        private Project(IProject entity, SyncStatus syncStatus)
        {
            Id = entity.Id;
            WorkspaceId = entity.WorkspaceId;
            ClientId = entity.ClientId;
            Name = entity.Name;
            IsPrivate = entity.IsPrivate;
            Active = entity.Active;
            At = entity.At;
            ServerDeletedAt = entity.ServerDeletedAt;
            Color = entity.Color;
            Billable = entity.Billable;
            Template = entity.Template;
            AutoEstimates = entity.AutoEstimates;
            EstimatedHours = entity.EstimatedHours;
            Rate = entity.Rate;
            Currency = entity.Currency;
            ActualHours = entity.ActualHours;
            SyncStatus = syncStatus;
        }

        public static Project Clean(IProject entity)
            => new Project(entity, SyncStatus.InSync);

        public static Project Dirty(IProject entity)
            => new Project(entity, SyncStatus.SyncNeeded);

        public static Project Unsyncable(IProject entity)
            => new Project(entity, SyncStatus.SyncFailed);
    }

    internal partial class Tag
    {
        private Tag(ITag entity, SyncStatus syncStatus)
        {
            Id = entity.Id;
            WorkspaceId = entity.WorkspaceId;
            Name = entity.Name;
            At = entity.At;
            SyncStatus = syncStatus;
        }

        public static Tag Clean(ITag entity)
            => new Tag(entity, SyncStatus.InSync);

        public static Tag Dirty(ITag entity)
            => new Tag(entity, SyncStatus.SyncNeeded);

        public static Tag Unsyncable(ITag entity)
            => new Tag(entity, SyncStatus.SyncFailed);
    }

    internal partial class Task
    {
        private Task(ITask entity, SyncStatus syncStatus)
        {
            Id = entity.Id;
            Name = entity.Name;
            ProjectId = entity.ProjectId;
            WorkspaceId = entity.WorkspaceId;
            UserId = entity.UserId;
            EstimatedSeconds = entity.EstimatedSeconds;
            Active = entity.Active;
            At = entity.At;
            TrackedSeconds = entity.TrackedSeconds;
            SyncStatus = syncStatus;
        }

        public static Task Clean(ITask entity)
            => new Task(entity, SyncStatus.InSync);

        public static Task Dirty(ITask entity)
            => new Task(entity, SyncStatus.SyncNeeded);

        public static Task Unsyncable(ITask entity)
            => new Task(entity, SyncStatus.SyncFailed);
    }

    internal partial class TimeEntry
    {
        private TimeEntry(ITimeEntry entity, SyncStatus syncStatus, bool isDeleted = false)
        {
            Id = entity.Id;
            WorkspaceId = entity.WorkspaceId;
            ProjectId = entity.ProjectId;
            TaskId = entity.TaskId;
            Billable = entity.Billable;
            Start = entity.Start;
            Stop = entity.Stop;
            Description = entity.Description;
            TagNames = entity.TagNames;
            TagIds = entity.TagIds;
            At = entity.At;
            ServerDeletedAt = entity.ServerDeletedAt;
            UserId = entity.UserId;
            CreatedWith = entity.CreatedWith;
            SyncStatus = syncStatus;
            IsDeleted = isDeleted;
        }

        public static TimeEntry Clean(ITimeEntry entity)
            => new TimeEntry(entity, SyncStatus.InSync);

        public static TimeEntry Dirty(ITimeEntry entity)
            => new TimeEntry(entity, SyncStatus.SyncNeeded);

        public static TimeEntry Unsyncable(ITimeEntry entity)
            => new TimeEntry(entity, SyncStatus.SyncFailed);

        public static TimeEntry CleanDeleted(ITimeEntry entity)
            => new TimeEntry(entity, SyncStatus.InSync, true);

        public static TimeEntry DirtyDeleted(ITimeEntry entity)
            => new TimeEntry(entity, SyncStatus.SyncNeeded, true);

        public static TimeEntry UnsyncableDeleted(ITimeEntry entity)
            => new TimeEntry(entity, SyncStatus.SyncFailed, true);
    }

    internal partial class User
    {
        private User(IUser entity, SyncStatus syncStatus)
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
            SyncStatus = syncStatus;
        }

        public static User Clean(IUser entity)
            => new User(entity, SyncStatus.InSync);

        public static User Dirty(IUser entity)
            => new User(entity, SyncStatus.SyncNeeded);

        public static User Unsyncable(IUser entity)
            => new User(entity, SyncStatus.SyncFailed);
    }

    internal partial class Workspace
    {
        private Workspace(IWorkspace entity, SyncStatus syncStatus)
        {
            Id = entity.Id;
            Name = entity.Name;
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
            SyncStatus = syncStatus;
        }

        public static Workspace Clean(IWorkspace entity)
            => new Workspace(entity, SyncStatus.InSync);

        public static Workspace Dirty(IWorkspace entity)
            => new Workspace(entity, SyncStatus.SyncNeeded);

        public static Workspace Unsyncable(IWorkspace entity)
            => new Workspace(entity, SyncStatus.SyncFailed);
    }
}