using System;
using Toggl.PrimeRadiant;
using System.Collections.Generic;
using Toggl.PrimeRadiant.Models;

namespace Toggl.Foundation.Models
{
    internal partial class TimeEntry
    {
        internal sealed class Builder
        {
            private const string errorMessage = "You need to set the {0} before building a time entry";

            public static Builder Create(long id) => new Builder(id);

            public long Id { get; }

            public SyncStatus SyncStatus { get; private set; }

            public bool Billable { get; private set; }

            public long? ProjectId { get; private set; }

            public string Description { get; private set; }

            public DateTimeOffset Start { get; private set; }

            public DateTimeOffset? Stop { get; private set; }

            public long? WorkspaceId { get; private set; }

            public long? TaskId { get; private set; }

            public List<long> TagIds { get; private set; }
                = new List<long>();

            public DateTimeOffset? At { get; private set; }

            public DateTimeOffset? ServerDeletedAt { get; private set; }

            public long? UserId { get; private set; }

            public string CreatedWith { get; private set; }

            public bool IsDeleted { get; private set; }

            private Builder(long id)
            {
                Id = id;
            }

            public TimeEntry Build()
            {
                ensureValidity();
                return new TimeEntry(this);
            }

            public Builder SetStart(DateTimeOffset start)
            {
                Start = start;
                return this;
            }

            public Builder SetSyncStatus(SyncStatus syncStatus)
            {
                SyncStatus = syncStatus;
                return this;
            }

            public Builder SetDescription(string description)
            {
                Description = description;
                return this;
            }

            public Builder SetBillable(bool billable)
            {
                Billable = billable; 
                return this;
            }

            internal Builder SetProjectId(long? projectId)
            {
                ProjectId = projectId;
                return this;
            }

            public Builder SetStop(DateTimeOffset? stop)
            {
                Stop = stop;
                return this;
            }

            public Builder SetWorkspaceId(long workspaceId)
            {
                WorkspaceId = workspaceId;
                return this;
            }

            public Builder SetTaskId(long? taskId)
            {
                TaskId = taskId;
                return this;
            }

            public Builder SetTagIds(IEnumerable<long> tagIds)
            {
                if (tagIds == null) return this;
                TagIds.Clear();
                TagIds.AddRange(tagIds);
                return this;
            }

            public Builder SetAt(DateTimeOffset at)
            {
                At = at;
                return this;
            }

            public Builder SetServerDeletedAt(DateTimeOffset? serverDeleteAt)
            {
                ServerDeletedAt = serverDeleteAt;
                return this;
            }

            public Builder SetUserId(long userId)
            {
                UserId = userId;
                return this;
            }

            public Builder SetCreatedWith(string createdWith)
            {
                CreatedWith = createdWith;
                return this;
            }

            public Builder SetIsDeleted(bool isDeleted)
            {
                IsDeleted = isDeleted;
                return this;
            }

            private void ensureValidity()
            {
                if (Start == default(DateTimeOffset))
                    throw new InvalidOperationException(string.Format(errorMessage, "start date"));

                if (Description == null)
                    throw new InvalidOperationException(string.Format(errorMessage, "description"));

                if (WorkspaceId == null || WorkspaceId == 0)
                    throw new InvalidOperationException(string.Format(errorMessage, "workspace id"));

                if (UserId == null || UserId == 0)
                    throw new InvalidOperationException(string.Format(errorMessage, "user id"));

                if (At == null)
                    throw new InvalidOperationException(string.Format(errorMessage, "at"));

                if (Start > Stop)
                    throw new InvalidOperationException("The stop date must be equal to or greater than the start date");
            }
        }

        public TimeEntry(IDatabaseTimeEntry timeEntry, DateTimeOffset stop)
            : this(timeEntry, SyncStatus.SyncNeeded, null)
        {
            if (Start > stop)
                throw new ArgumentOutOfRangeException(nameof(stop), "The stop date must be equal to or greater than the start date");

            Stop = stop;
        }

        private TimeEntry(Builder builder)
        {
            Id = builder.Id;
            Stop = builder.Stop;
            At = builder.At.Value;
            Start = builder.Start;
            TagIds = builder.TagIds;
            TaskId = builder.TaskId;
            Billable = builder.Billable;
            UserId = builder.UserId.Value;
            IsDeleted = builder.IsDeleted;
            ProjectId = builder.ProjectId;
            SyncStatus = builder.SyncStatus;
            CreatedWith = builder.CreatedWith;
            Description = builder.Description;
            WorkspaceId = builder.WorkspaceId.Value;
            ServerDeletedAt = builder.ServerDeletedAt;
        }
    }

    internal static class TimeEntryExtensions
    {
        public static TimeEntry With(this IDatabaseTimeEntry self, DateTimeOffset stop) => new TimeEntry(self, stop);
    }
}
