using System;
using FsCheck.Xunit;
using Xunit;
using FluentAssertions;
using Toggl.Foundation.Sync.ConflictResolution;
using Toggl.PrimeRadiant;
using Toggl.Foundation.Sync.ConflictResolution.Selectors;

namespace Toggl.Foundation.Tests.Sync.ConflictResolution
{
    public sealed class PreferNewerTests
    {
        [Fact]
        public void ThrowsWhenIncomingEntityIsNull()
        {
            var existingEntity = new TestModel();

            Action resolving = () => resolver.Resolve(null, null);
            Action resolvingWithExistingClient = () => resolver.Resolve(existingEntity, null);

            resolving.ShouldThrow<ArgumentNullException>();
            resolvingWithExistingClient.ShouldThrow<ArgumentNullException>();
        }

        [Property]
        public void IgnoreOutdatedIncomingChange(DateTimeOffset existing, DateTimeOffset incoming)
        {
            if (existing <= incoming) return;
            var existingEntity = new TestModel(existing);
            var incomingEntity = new TestModel(incoming);

            var mode = resolver.Resolve(existingEntity, incomingEntity);

            mode.Should().Be(ConflictResolutionMode.Ignore);
        }

        [Property]
        public void UpdateWhenIncomingChangeIsNewerThanExising(DateTimeOffset existing, DateTimeOffset incoming)
        {
            if (existing > incoming) return;
            var existingEntity = new TestModel(existing);
            var incomingEntity = new TestModel(incoming);

            var mode = resolver.Resolve(existingEntity, incomingEntity);

            mode.Should().Be(ConflictResolutionMode.Update);
        }

        [Property]
        public void CreateNewWhenThereIsNoExistingEntity(DateTimeOffset at)
        {
            var incomingEntity = new TestModel(at);

            var mode = resolver.Resolve(null, incomingEntity);

            mode.Should().Be(ConflictResolutionMode.Create);
        }

        [Property]
        public void DeleteWhenTheIncomingDataHasSomeServerDeletedAt(DateTimeOffset existing, DateTimeOffset incoming, DateTimeOffset serverDeletedAt)
        {
            var existingEntity = new TestModel(existing);
            var incomingEntity = new TestModel(incoming, serverDeletedAt);

            var mode = resolver.Resolve(existingEntity, incomingEntity);

            mode.Should().Be(ConflictResolutionMode.Delete);
        }

        [Property]
        public void IgnoreWhenTheIncomingDataHasSomeServerDeletedAtButThereIsNoExistingEntity(DateTimeOffset incoming, DateTimeOffset serverDeletedAt)
        {
            var incomingEntity = new TestModel(incoming, serverDeletedAt);

            var mode = resolver.Resolve(null, incomingEntity);

            mode.Should().Be(ConflictResolutionMode.Ignore);
        }

        private sealed class TestModel : IDatabaseSyncable
        {
            public bool IsDirty { get; }
            public DateTimeOffset At { get; }
            public DateTimeOffset? ServerDeletedAt { get; }

            public TestModel() : this(null, null) { }

            public TestModel(DateTimeOffset at) : this(at, null) { }

            public TestModel(DateTimeOffset? at, DateTimeOffset? deleted)
            {
                At = at ?? DateTimeOffset.Now;
                ServerDeletedAt = deleted;
            }
        }

        private sealed class TestModelSelector : ISyncSelector<TestModel>
        {
            public DateTimeOffset LastModified(TestModel model)
                => model.At;

            public bool IsDeleted(TestModel model)
                => model.ServerDeletedAt.HasValue;
        }

        private IConflictResolver<TestModel> resolver { get; } = new PreferNewer<TestModel>(new TestModelSelector());
    }
}
