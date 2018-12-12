using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Microsoft.Reactive.Testing;
using NSubstitute;
using Toggl.Foundation.Sync;
using Toggl.Foundation.Sync.Helpers;
using Toggl.Multivac.Models;
using Toggl.PrimeRadiant.Models;
using Toggl.Ultrawave.ApiClients;
using Xunit;

namespace Toggl.Foundation.Tests.Sync.Helpers
{
    public class RateLimitingAwareRequestSenderTests
    {
        public abstract class BaseRateLimitingAwareRequestSenderTest
        {
            protected ITimeService TimeService { get; } = Substitute.For<ITimeService>();
            protected ILeakyBucket LeakyBucket { get; } = Substitute.For<ILeakyBucket>();
            protected TestScheduler Scheduler { get; } = new TestScheduler();
            protected IRequestSender InternalRequestSender { get; } = Substitute.For<IRequestSender>();
            protected IRequestSender RequestSender { get; }

            protected BaseRateLimitingAwareRequestSenderTest()
            {
                RequestSender = new RateLimitingAwareRequestSender(TimeService, LeakyBucket, Scheduler, InternalRequestSender);
            }

            protected void PrepareOneTwoThreeSecondDelays()
            {
                LeakyBucket.TryClaimFreeSlot(Arg.Any<DateTimeOffset>(), out _)
                    .Returns(
                        x =>
                        {
                            x[1] = TimeSpan.FromSeconds(1);
                            return false;
                        },
                        x =>
                        {
                            x[1] = TimeSpan.FromSeconds(2);
                            return false;
                        },
                        x =>
                        {
                            x[1] = TimeSpan.FromSeconds(3);
                            return false;
                        },
                        x => true);
            }
        }

        public sealed class TheFetchMethod : BaseRateLimitingAwareRequestSenderTest
        {
            private readonly IUserApi api = Substitute.For<IUserApi>();

            public TheFetchMethod()
            {
                InternalRequestSender.Fetch<IUser, IUserApi>(api)
                    .Returns(Observable.Return<IUser>(null));
            }

            [Fact]
            public async Task SendsTheRequestWhenThereIsAFreeSlot()
            {
                LeakyBucket.TryClaimFreeSlot(Arg.Any<DateTimeOffset>(), out _).Returns(true);

                await RequestSender.Fetch<IUser, IUserApi>(api);

                InternalRequestSender.Received().Fetch<IUser, IUserApi>(api);
            }

            [Fact]
            public void DelaysSendingTheRequestByTheGivenDelays()
            {
                PrepareOneTwoThreeSecondDelays();

                RequestSender.Fetch<IUser, IUserApi>(api).Subscribe();

                Scheduler.AdvanceBy(TimeSpan.FromSeconds(6).Ticks - 1);
                InternalRequestSender.DidNotReceive().Fetch<IUser, IUserApi>(Arg.Any<IUserApi>());
                Scheduler.AdvanceBy(1);
                InternalRequestSender.Received().Fetch<IUser, IUserApi>(api);
            }
        }

        public sealed class TheFetchAllMethod : BaseRateLimitingAwareRequestSenderTest
        {
            private readonly IWorkspacesApi api = Substitute.For<IWorkspacesApi>();

            public TheFetchAllMethod()
            {
                InternalRequestSender.FetchAll<IWorkspace, IWorkspacesApi>(api)
                    .Returns(Observable.Return<List<IWorkspace>>(null));
            }

            [Fact]
            public async Task SendsTheRequestWhenThereIsAFreeSlot()
            {
                LeakyBucket.TryClaimFreeSlot(Arg.Any<DateTimeOffset>(), out _).Returns(true);

                await RequestSender.FetchAll<IWorkspace, IWorkspacesApi>(api);

                InternalRequestSender.Received().FetchAll<IWorkspace, IWorkspacesApi>(api);
            }

            [Fact]
            public void DelaysSendingTheRequestByTheGivenDelays()
            {
                PrepareOneTwoThreeSecondDelays();

                RequestSender.FetchAll<IWorkspace, IWorkspacesApi>(api).Subscribe();

                Scheduler.AdvanceBy(TimeSpan.FromSeconds(6).Ticks - 1);
                InternalRequestSender.DidNotReceive().FetchAll<IWorkspace, IWorkspacesApi>(Arg.Any<IWorkspacesApi>());
                Scheduler.AdvanceBy(1);
                InternalRequestSender.Received().FetchAll<IWorkspace, IWorkspacesApi>(api);
            }
        }

        public sealed class TheFetchAllSinceIfPossibleMethod : BaseRateLimitingAwareRequestSenderTest
        {
            private readonly ITagsApi api = Substitute.For<ITagsApi>();

            public TheFetchAllSinceIfPossibleMethod()
            {
                InternalRequestSender.FetchAllSinceIfPossible<ITag, IDatabaseTag, ITagsApi>(api)
                    .Returns(Observable.Return<List<ITag>>(null));
            }

            [Fact]
            public async Task SendsTheRequestWhenThereIsAFreeSlot()
            {
                LeakyBucket.TryClaimFreeSlot(Arg.Any<DateTimeOffset>(), out _).Returns(true);

                await RequestSender.FetchAllSinceIfPossible<ITag, IDatabaseTag, ITagsApi>(api);

                InternalRequestSender.Received().FetchAllSinceIfPossible<ITag, IDatabaseTag, ITagsApi>(api);
            }

            [Fact]
            public void DelaysSendingTheRequestByTheGivenDelays()
            {
                PrepareOneTwoThreeSecondDelays();

                RequestSender.FetchAllSinceIfPossible<ITag, IDatabaseTag, ITagsApi>(api).Subscribe();

                Scheduler.AdvanceBy(TimeSpan.FromSeconds(6).Ticks - 1);
                InternalRequestSender.DidNotReceive().FetchAllSinceIfPossible<ITag, IDatabaseTag, ITagsApi>(Arg.Any<ITagsApi>());
                Scheduler.AdvanceBy(1);
                InternalRequestSender.Received().FetchAllSinceIfPossible<ITag, IDatabaseTag, ITagsApi>(api);
            }
        }

        public sealed class TheFetchTimeEntriesMethod : BaseRateLimitingAwareRequestSenderTest
        {
            private readonly ITimeEntriesApi api = Substitute.For<ITimeEntriesApi>();

            public TheFetchTimeEntriesMethod()
            {
                InternalRequestSender.FetchTimeEntries(api)
                    .Returns(Observable.Return<List<ITimeEntry>>(null));
            }

            [Fact]
            public async Task SendsTheRequestWhenThereIsAFreeSlot()
            {
                LeakyBucket.TryClaimFreeSlot(Arg.Any<DateTimeOffset>(), out _).Returns(true);

                await RequestSender.FetchTimeEntries(api);

                InternalRequestSender.Received().FetchTimeEntries(api);
            }

            [Fact]
            public void DelaysSendingTheRequestByTheGivenDelays()
            {
                PrepareOneTwoThreeSecondDelays();

                RequestSender.FetchTimeEntries(api).Subscribe();

                Scheduler.AdvanceBy(TimeSpan.FromSeconds(6).Ticks - 1);
                InternalRequestSender.DidNotReceive().FetchTimeEntries(Arg.Any<ITimeEntriesApi>());
                Scheduler.AdvanceBy(1);
                InternalRequestSender.Received().FetchTimeEntries(api);
            }
        }
    }
}
