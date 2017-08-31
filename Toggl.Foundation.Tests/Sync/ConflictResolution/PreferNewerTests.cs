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
            Action resolvingWithExistingLocalEntity = () => resolver.Resolve(existingEntity, null);

            resolving.ShouldThrow<ArgumentNullException>();
            resolvingWithExistingLocalEntity.ShouldThrow<ArgumentNullException>();
        }

        [Property]
        public void IgnoreOutdatedIncomingDataWhenLocalEntityIsDirty(DateTimeOffset existing, DateTimeOffset incoming)
        {
            if (existing <= incoming)
                (existing, incoming) = (incoming, existing);
            var existingEntity = new TestModel(existing, dirty: true);
            var incomingEntity = new TestModel(incoming);

            var mode = resolver.Resolve(existingEntity, incomingEntity);

            mode.Should().Be(ConflictResolutionMode.Ignore);
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
        public void DeleteWhenTheIncomingDataHasSomeServerDeletedAtEvenWhenLocalEntityIsDirty(DateTimeOffset existing, DateTimeOffset incoming, DateTimeOffset serverDeletedAt)
        {
            var existingEntity = new TestModel(existing, dirty: true);
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

        [Property]
        public void ThrowAwayIncommingIfUserMadeChangesLocally(DateTimeOffset existing, int seed)
        {
            DateTimeOffset incoming = existing.Add(randomTimeSpan(seed, resolver.MarginOfError.TotalSeconds));
            var existingEntity = new TestModel(existing, dirty: true);
            var incomingEntity = new TestModel(incoming);

            var mode = resolver.Resolve(existingEntity, incomingEntity);

            mode.Should().Be(ConflictResolutionMode.Ignore);
        }

        [Property]
        public void UpdateWhenIncomingChangeIsNewerThanExising(DateTimeOffset existing, DateTimeOffset incoming)
        {
            if (existing > incoming)
                (existing, incoming) = (incoming, existing);
            var existingEntity = new TestModel(existing, dirty: true);
            var incomingEntity = new TestModel(incoming);

            var mode = resolver.Resolve(existingEntity, incomingEntity);

            mode.Should().Be(ConflictResolutionMode.Update);
        }

        [Property]
        public void UpdateWhenIncomingChangeIsNewerThanExisingConsideringTheMarginOfError(DateTimeOffset existing, DateTimeOffset incoming)
        {
            if (existing > incoming.Subtract(resolver.MarginOfError))
                (existing, incoming) = (incoming, existing);
            var existingEntity = new TestModel(existing, dirty: true);
            var incomingEntity = new TestModel(incoming);

            var mode = resolver.Resolve(existingEntity, incomingEntity);

            mode.Should().Be(ConflictResolutionMode.Update);
        }

        [Property]
        public void UpdateIfUserMadeChangesLocallyRecentlyButThereIsNoMarginOfError(DateTimeOffset existing, int seed)
        {
            DateTimeOffset incoming = existing.Add(randomTimeSpan(seed, 1));
            var existingEntity = new TestModel(existing, dirty: true);
            var incomingEntity = new TestModel(incoming);

            var mode = zeroMarginOfErrorResolver.Resolve(existingEntity, incomingEntity);

            mode.Should().Be(ConflictResolutionMode.Update);
        }

        [Property]
        public void AlwaysOverrideNonDeletedCleanEntity(DateTimeOffset existing, DateTimeOffset incoming)
        {
            if (existing <= incoming)
                (existing, incoming) = (incoming, existing);

            var existingEntity = new TestModel(existing, dirty: false);
            var incomingEntity = new TestModel(incoming, deleted: null);

            var mode = resolver.Resolve(existingEntity, incomingEntity);

            mode.Should().Be(ConflictResolutionMode.Update);
        }

        private sealed class TestModel : IDatabaseSyncable
        {
            public bool IsDirty { get; }
            public DateTimeOffset At { get; }
            public DateTimeOffset? ServerDeletedAt { get; }

            public TestModel(DateTimeOffset? at = null, DateTimeOffset? deleted = null, bool dirty = false)
            {
                At = at ?? DateTimeOffset.Now;
                ServerDeletedAt = deleted;
                IsDirty = dirty;
            }
        }

        private sealed class TestModelSelector : ISyncSelector<TestModel>
        {
            public DateTimeOffset LastModified(TestModel model)
                => model.At;

            public bool IsDirty(TestModel model)
                => model.IsDirty;

            public bool IsDeleted(TestModel model)
                => model.ServerDeletedAt.HasValue;
        }

        private TimeSpan randomTimeSpan(int seed, double max)
        {
            var lessThanMarginOfErrorSeconds = (new Random(seed)).NextDouble() * max;
            return TimeSpan.FromSeconds(lessThanMarginOfErrorSeconds);
        }

        private IConflictResolver<TestModel> resolver { get; }
            = new PreferNewer<TestModel>(new TestModelSelector(), TimeSpan.FromSeconds(5));

        private IConflictResolver<TestModel> zeroMarginOfErrorResolver { get; }
            = new PreferNewer<TestModel>(new TestModelSelector());
    }
}
