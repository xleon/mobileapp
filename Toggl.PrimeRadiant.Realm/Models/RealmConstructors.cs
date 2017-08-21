﻿using Realms;
using System.Linq;
using Toggl.Multivac.Models;
using Toggl.PrimeRadiant.Models;

namespace Toggl.PrimeRadiant.Realm
{
    internal partial class RealmClient
    {
        [PrimaryKey]
        public long Id { get; set; }

        public bool IsDirty { get; set; }

        public RealmClient() { }

        public RealmClient(IDatabaseClient entity, Realms.Realm realm)
            : this(entity as IClient, realm)
        {
            IsDirty = entity.IsDirty;
        }

        public RealmClient(IClient entity, Realms.Realm realm)
        {
            Id = entity.Id;
            var skipWorkspaceFetch = entity?.WorkspaceId == null || entity.WorkspaceId == 0;
            RealmWorkspace = skipWorkspaceFetch ? null : realm.All<RealmWorkspace>().Single(x => x.Id == entity.WorkspaceId);
            Name = entity.Name;
            At = entity.At;
            ServerDeletedAt = entity.ServerDeletedAt;
            IsDirty = true;
        }
    }

    internal partial class RealmProject
    {
        [PrimaryKey]
        public long Id { get; set; }

        public bool IsDirty { get; set; }

        public RealmProject() { }

        public RealmProject(IDatabaseProject entity, Realms.Realm realm)
            : this(entity as IProject, realm)
        {
            IsDirty = entity.IsDirty;
        }

        public RealmProject(IProject entity, Realms.Realm realm)
        {
            Id = entity.Id;
            var skipWorkspaceFetch = entity?.WorkspaceId == null || entity.WorkspaceId == 0;
            RealmWorkspace = skipWorkspaceFetch ? null : realm.All<RealmWorkspace>().Single(x => x.Id == entity.WorkspaceId);
            var skipClientFetch = entity?.ClientId == null || entity.ClientId == 0;
            RealmClient = skipClientFetch ? null : realm.All<RealmClient>().Single(x => x.Id == entity.ClientId);
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
            IsDirty = true;
        }
    }

    internal partial class RealmTag
    {
        [PrimaryKey]
        public long Id { get; set; }

        public bool IsDirty { get; set; }

        public RealmTag() { }

        public RealmTag(IDatabaseTag entity, Realms.Realm realm)
            : this(entity as ITag, realm)
        {
            IsDirty = entity.IsDirty;
        }

        public RealmTag(ITag entity, Realms.Realm realm)
        {
            Id = entity.Id;
            var skipWorkspaceFetch = entity?.WorkspaceId == null || entity.WorkspaceId == 0;
            RealmWorkspace = skipWorkspaceFetch ? null : realm.All<RealmWorkspace>().Single(x => x.Id == entity.WorkspaceId);
            Name = entity.Name;
            At = entity.At;
            IsDirty = true;
        }
    }

    internal partial class RealmTask
    {
        [PrimaryKey]
        public long Id { get; set; }

        public bool IsDirty { get; set; }

        public RealmTask() { }

        public RealmTask(IDatabaseTask entity, Realms.Realm realm)
            : this(entity as ITask, realm)
        {
            IsDirty = entity.IsDirty;
        }

        public RealmTask(ITask entity, Realms.Realm realm)
        {
            Id = entity.Id;
            Name = entity.Name;
            var skipProjectFetch = entity?.ProjectId == null || entity.ProjectId == 0;
            RealmProject = skipProjectFetch ? null : realm.All<RealmProject>().Single(x => x.Id == entity.ProjectId);
            var skipWorkspaceFetch = entity?.WorkspaceId == null || entity.WorkspaceId == 0;
            RealmWorkspace = skipWorkspaceFetch ? null : realm.All<RealmWorkspace>().Single(x => x.Id == entity.WorkspaceId);
            var skipUserFetch = entity?.UserId == null || entity.UserId == 0;
            RealmUser = skipUserFetch ? null : realm.All<RealmUser>().Single(x => x.Id == entity.UserId);
            EstimatedSeconds = entity.EstimatedSeconds;
            Active = entity.Active;
            At = entity.At;
            TrackedSeconds = entity.TrackedSeconds;
            IsDirty = true;
        }
    }

    internal partial class RealmTimeEntry
    {
        [PrimaryKey]
        public long Id { get; set; }

        public bool IsDirty { get; set; }

        public RealmTimeEntry() { }

        public RealmTimeEntry(IDatabaseTimeEntry entity, Realms.Realm realm)
            : this(entity as ITimeEntry, realm)
        {
            IsDirty = entity.IsDirty;
        }

        public RealmTimeEntry(ITimeEntry entity, Realms.Realm realm)
        {
            Id = entity.Id;
            var skipWorkspaceFetch = entity?.WorkspaceId == null || entity.WorkspaceId == 0;
            RealmWorkspace = skipWorkspaceFetch ? null : realm.All<RealmWorkspace>().Single(x => x.Id == entity.WorkspaceId);
            var skipProjectFetch = entity?.ProjectId == null || entity.ProjectId == 0;
            RealmProject = skipProjectFetch ? null : realm.All<RealmProject>().Single(x => x.Id == entity.ProjectId);
            var skipTaskFetch = entity?.TaskId == null || entity.TaskId == 0;
            RealmTask = skipTaskFetch ? null : realm.All<RealmTask>().Single(x => x.Id == entity.TaskId);
            Billable = entity.Billable;
            Start = entity.Start;
            Stop = entity.Stop;
            Description = entity.Description;
            TagNames = entity.TagNames;
            TagIds = entity.TagIds;
            At = entity.At;
            ServerDeletedAt = entity.ServerDeletedAt;
            var skipUserFetch = entity?.UserId == null || entity.UserId == 0;
            RealmUser = skipUserFetch ? null : realm.All<RealmUser>().Single(x => x.Id == entity.UserId);
            CreatedWith = entity.CreatedWith;
            IsDirty = true;
        }
    }

    internal partial class RealmUser
    {
        [PrimaryKey]
        public long Id { get; set; }

        public bool IsDirty { get; set; }

        public RealmUser() { }

        public RealmUser(IDatabaseUser entity, Realms.Realm realm)
            : this(entity as IUser, realm)
        {
            IsDirty = entity.IsDirty;
        }

        public RealmUser(IUser entity, Realms.Realm realm)
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
        public long Id { get; set; }

        public bool IsDirty { get; set; }

        public RealmWorkspace() { }

        public RealmWorkspace(IDatabaseWorkspace entity, Realms.Realm realm)
            : this(entity as IWorkspace, realm)
        {
            IsDirty = entity.IsDirty;
        }

        public RealmWorkspace(IWorkspace entity, Realms.Realm realm)
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
            IsDirty = true;
        }
    }
}