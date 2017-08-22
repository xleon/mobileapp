using System;
using FluentAssertions;
using Toggl.Ultrawave.Models;
using Xunit;

namespace Toggl.Ultrawave.Tests.Models
{
    public class TimeEntryTests
    {
        public sealed class TheTimeEntryModel
        {
            private string validJson
                => "{\"id\":525144694,\"workspace_id\":1414373,\"project_id\":3178352,\"task_id\":null,\"billable\":false,\"start\":\"2017-04-25T19:34:39+00:00\",\"stop\":null,\"description\":\"Some short description\",\"tags\":[\"one\",\"test\",\"two\"],\"tag_ids\":[313040,3129041,319042],\"at\":\"2017-04-25T20:12:27+00:00\",\"server_deleted_at\":null,\"user_id\":0,\"created_with\":\"SomeApp\"}";

            private TimeEntry validTimeEntry => new TimeEntry
            {
                Id = 525144694,
                WorkspaceId = 1414373,
                ProjectId = 3178352,
                TaskId = null,
                Billable = false,
                Start = new DateTimeOffset(2017, 4, 25, 19, 34, 39, TimeSpan.Zero),
                Stop = null,
                Description = "Some short description",
                TagNames = new string[] { "one", "test", "two" },
                TagIds = new long[] { 313040, 3129041, 319042 },
                At = new DateTimeOffset(2017, 4, 25, 20, 12, 27, TimeSpan.Zero),
                ServerDeletedAt = null,
                UserId = 0,
                CreatedWith = "SomeApp"
            };

            [Fact]
            public void HasConstructorWhichCopiesValuesFromInterfaceToTheNewInstance()
            {
                var clonedObject = new TimeEntry(validTimeEntry);

                clonedObject.Should().NotBeSameAs(validTimeEntry);
                clonedObject.ShouldBeEquivalentTo(validTimeEntry, options => options.IncludingProperties());
            }

            [Fact]
            public void CanBeDeserialized()
            {
                SerializationHelper.CanBeDeserialized(validJson, validTimeEntry);
            }

            [Fact]
            public void CanBeSerialized()
            {
                SerializationHelper.CanBeSerialized(validJson, validTimeEntry);
            }
        }
    }
}
