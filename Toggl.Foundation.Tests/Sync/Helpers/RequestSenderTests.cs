using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using FsCheck.Xunit;
using NSubstitute;
using Toggl.Foundation.Sync.Helpers;
using Toggl.Multivac.Models;
using Toggl.PrimeRadiant;
using Toggl.PrimeRadiant.Models;
using Toggl.Ultrawave.ApiClients;
using Xunit;

namespace Toggl.Foundation.Tests.Sync.Helpers
{
    public sealed class RequestSenderTests
    {
        public abstract class BaseRequestSenderTests
        {
            protected DateTimeOffset Now { get; } = new DateTimeOffset(2017, 02, 15, 13, 50, 00, TimeSpan.Zero);
            protected ITimeService TimeService { get; } = Substitute.For<ITimeService>();
            protected ITogglDatabase Database { get; } = Substitute.For<ITogglDatabase>();
            protected IRequestSender Sender { get; }

            protected BaseRequestSenderTests()
            {
                TimeService.CurrentDateTime.Returns(Now);
                Sender = new RequestSender(Database, TimeService);
            }
        }

        public sealed class TheFetchMethod : BaseRequestSenderTests
        {
            private readonly IUserApi api = Substitute.For<IUserApi>();

            [Fact, LogIfTooSlow]
            public async Task ReturnsAConnectedObservable()
            {
                var called = false;

                api.Get().Returns(Observable.Start<IUser>(() => { called = true; return null; }));

                await Sender.Fetch<IUser, IUserApi>(api).FirstAsync();

                called.Should().BeTrue();
            }

            [Fact, LogIfTooSlow]
            public void ApiRequestIsSentOnceNoMatterHowManyObserversSubscribe()
            {
                api.Get().Returns(Observable.Return<IUser>(null));

                var observable = Sender.Fetch<IUser, IUserApi>(api);
                observable.Subscribe();
                observable.Subscribe();

                api.Received(1).Get();
            }
        }

        public sealed class TheFetchAllMethod : BaseRequestSenderTests
        {
            private readonly IWorkspacesApi api = Substitute.For<IWorkspacesApi>();

            [Fact, LogIfTooSlow]
            public async Task ReturnsAConnectedObservable()
            {
                var called = false;

                api.GetAll().Returns(Observable.Start<List<IWorkspace>>(() => { called = true; return null; }));

                await Sender.FetchAll<IWorkspace, IWorkspacesApi>(api);

                called.Should().BeTrue();
            }

            [Fact, LogIfTooSlow]
            public void ApiRequestIsSentOnceNoMatterHowManyObserversSubscribe()
            {
                api.GetAll().Returns(Observable.Return<List<IWorkspace>>(null));

                var observable = Sender.FetchAll<IWorkspace, IWorkspacesApi>(api);
                observable.Subscribe();
                observable.Subscribe();

                api.Received(1).GetAll();
            }
        }

        public class TheFetchTimeEntriesMethod : BaseRequestSenderTests
        {
            private readonly ITimeEntriesApi api = Substitute.For<ITimeEntriesApi>();

            [Property]
            public void MakesCorrectCallsWithSinceThresholdsWhenSinceIsLessThanTwoMonthsInThePast(int seed)
            {
                var rnd = new Random(seed);
                var percent = rnd.NextDouble();

                var now = TimeService.CurrentDateTime;
                var twoMonths = (now.AddMonths(2) - now);
                var seconds = twoMonths.TotalSeconds * percent;
                var since = now.AddSeconds(-seconds);

                var sinceParameters = Substitute.For<ISinceParameterRepository>();
                sinceParameters.Get<IDatabaseTimeEntry>().Returns(since);
                Database.SinceParameters.Returns(sinceParameters);

                Sender.FetchTimeEntries(api);

                api.Received().GetAllSince(since);
            }

            [Fact, LogIfTooSlow]
            public async Task FetchesTwoMonthsOfTimeEntriesDataIncludingTwoDaysAfterNow()
            {
                var sinceParameters = Substitute.For<ISinceParameterRepository>();
                sinceParameters.Get<IDatabaseTimeEntry>().Returns(Now.AddMonths(-8));
                Database.SinceParameters.Returns(sinceParameters);

                await Sender.FetchTimeEntries(api);

                var max = TimeSpan.FromDays(62);
                var min = TimeSpan.FromDays(59);

                await api.Received().GetAll(
                    Arg.Is<DateTimeOffset>(start => min <= Now - start && Now - start <= max), Arg.Is(Now.AddDays(2)));
            }

            [Fact, LogIfTooSlow]
            public async Task SendsTheRequestRightAway()
            {
                var called = false;

                api.GetAll(Arg.Any<DateTimeOffset>(), Arg.Any<DateTimeOffset>())
                    .Returns(Observable.Start<List<ITimeEntry>>(() => { called = true; return null; }));

                await Sender.FetchTimeEntries(api);

                called.Should().BeTrue();
            }

            [Fact, LogIfTooSlow]
            public void ApiRequestIsSentOnceNoMatterHowManyObserversSubscribe()
            {
                api.GetAll(Arg.Any<DateTimeOffset>(), Arg.Any<DateTimeOffset>())
                    .Returns(Observable.Return<List<ITimeEntry>>(null));

                var observable = Sender.FetchTimeEntries(api);
                observable.Subscribe();
                observable.Subscribe();

                api.Received(1).GetAll(Arg.Any<DateTimeOffset>(), Arg.Any<DateTimeOffset>());
            }
        }

        public sealed class TheFetchAppSinceIfPossibleMethod : BaseRequestSenderTests
        {
            private readonly ITagsApi api = Substitute.For<ITagsApi>();

            [Property]
            public void MakesCorrectCallsWithSinceThresholdsWhenSinceIsLessThanTwoMonthsInThePast(int seed)
            {
                var rnd = new Random(seed);
                var percent = rnd.NextDouble();

                var now = TimeService.CurrentDateTime;
                var twoMonths = (now.AddMonths(2) - now);
                var seconds = twoMonths.TotalSeconds * percent;
                var since = now.AddSeconds(-seconds);

                var sinceParameters = Substitute.For<ISinceParameterRepository>();
                sinceParameters.Get<IDatabaseTag>().Returns(since);
                Database.SinceParameters.Returns(sinceParameters);

                Sender.FetchAllSinceIfPossible<ITag, IDatabaseTag, ITagsApi>(api);

                api.Received().GetAllSince(since);
            }

            [Fact, LogIfTooSlow]
            public void MakesApiCallsWithoutTheSinceParameterWhenTheThresholdIsMoreThanTwoMonthsInThePast()
            {
                var now = TimeService.CurrentDateTime;

                var sinceParameters = Substitute.For<ISinceParameterRepository>();
                sinceParameters.Get<IDatabaseTag>().Returns(now.AddMonths(-6));
                Database.SinceParameters.Returns(sinceParameters);

                Sender.FetchAllSinceIfPossible<ITag, IDatabaseTag, ITagsApi>(api);

                api.Received().GetAll();
            }

            [Fact, LogIfTooSlow]
            public async Task SendsTheRequestRightAway()
            {
                var called = false;
                var sinceParameters = Substitute.For<ISinceParameterRepository>();
                sinceParameters.Get<IDatabaseTag>().Returns(Now.AddDays(-1));
                Database.SinceParameters.Returns(sinceParameters);
                api.GetAllSince(Arg.Any<DateTimeOffset>())
                    .Returns(Observable.Start<List<ITag>>(() => { called = true; return null; }));

                await Sender.FetchAllSinceIfPossible<ITag, IDatabaseTag, ITagsApi>(api);

                called.Should().BeTrue();
            }

            [Fact, LogIfTooSlow]
            public void ApiRequestIsSentOnceNoMatterHowManyObserversSubscribe()
            {
                api.GetAllSince(Arg.Any<DateTimeOffset>()).Returns(Observable.Return<List<ITag>>(null));
                var sinceParameters = Substitute.For<ISinceParameterRepository>();
                sinceParameters.Get<IDatabaseTag>().Returns(Now.AddDays(-1));
                Database.SinceParameters.Returns(sinceParameters);

                var observable = Sender.FetchAllSinceIfPossible<ITag, IDatabaseTag, ITagsApi>(api);
                observable.Subscribe();
                observable.Subscribe();

                api.Received(1).GetAllSince(Arg.Any<DateTimeOffset>());
            }
        }
    }
}
