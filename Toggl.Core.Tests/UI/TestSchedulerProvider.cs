using System.Reactive.Concurrency;
using Microsoft.Reactive.Testing;
using Toggl.Shared;

namespace Toggl.Core.Tests.UI
{
    public sealed class TestSchedulerProvider : ISchedulerProvider
    {
        public readonly TestScheduler TestScheduler;

        public TestSchedulerProvider()
        {
            TestScheduler = new TestScheduler();
        }

        public IScheduler MainScheduler => TestScheduler;
        public IScheduler DefaultScheduler => TestScheduler;
        public IScheduler BackgroundScheduler => TestScheduler;
    }
}
