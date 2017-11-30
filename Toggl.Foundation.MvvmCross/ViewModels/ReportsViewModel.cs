using System;
using MvvmCross.Core.ViewModels;
using Toggl.Multivac;

namespace Toggl.Foundation.MvvmCross.ViewModels
{
    [Preserve(AllMembers = true)]
    public sealed class ReportsViewModel : MvxViewModel
    {
        private const string dateFormat = "dd MMM";

        private DateTimeOffset startDate;
        private DateTimeOffset endDate;

        private readonly ITimeService timeService;

        public bool HasData { get; }

        public string CurrentPeriodString
            => $"{startDate.ToString(dateFormat)} - {endDate.ToString(dateFormat)}";

        public ReportsViewModel(ITimeService timeService)
        {
            Ensure.Argument.IsNotNull(timeService, nameof(timeService));

            this.timeService = timeService;
        }

        public override void Prepare()
        {
            endDate = timeService.CurrentDateTime;
            startDate = endDate.AddDays(-7);
        }
    }
}
