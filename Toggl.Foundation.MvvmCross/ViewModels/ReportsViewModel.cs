using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using MvvmCross.Core.ViewModels;
using Toggl.Foundation.MvvmCross.Parameters;
using Toggl.Foundation.Reports;
using Toggl.Multivac;

namespace Toggl.Foundation.MvvmCross.ViewModels
{
    [Preserve(AllMembers = true)]
    public sealed class ReportsViewModel : MvxViewModel<long>
    {
        private const string dateFormat = "d MMM";

        private readonly ITimeService timeService;
        private readonly IReportsProvider reportsProvider;
        private readonly Subject<Unit> reportSubject = new Subject<Unit>();

        private IDisposable reportDisposable;
        private DateTimeOffset startDate;
        private DateTimeOffset endDate;
        private long workspaceId;

        public bool IsLoading { get; private set; }

        public TimeSpan TotalTime { get; private set; } = TimeSpan.Zero;

        public bool TotalTimeIsZero => TotalTime.Ticks == 0;

        public float? BillablePercentage { get; private set; } = null;

        public MvxObservableCollection<ChartSegment> Segments { get; } = new MvxObservableCollection<ChartSegment>();

        public bool ShowEmptyState => !Segments.Any() && !IsLoading;

        public string CurrentDateRangeString { get; private set; }

        public bool IsCurrentWeek
        {
            get
            {
                var currentDate = timeService.CurrentDateTime.Date;
                var startOfWeek = currentDate.AddDays(1 - (int)currentDate.DayOfWeek);
                var endOfWeek = startOfWeek.AddDays(6);

                return startDate.Date == startOfWeek
                       && endDate.Date == endOfWeek;
            }
        }

        public IMvxCommand<DateRangeParameter> ChangeDateRangeCommand { get; }

        public ReportsViewModel(IReportsProvider reportsProvider, ITimeService timeService)
        {
            Ensure.Argument.IsNotNull(reportsProvider, nameof(reportsProvider));
            Ensure.Argument.IsNotNull(timeService, nameof(timeService));

            this.timeService = timeService;
            this.reportsProvider = reportsProvider;

            ChangeDateRangeCommand = new MvxCommand<DateRangeParameter>(changeDateRange);
        }

        public override void Prepare(long parameter)
        {
            workspaceId = parameter;

            var currentDate = timeService.CurrentDateTime.Date;
            startDate = currentDate.AddDays(1 - (int)currentDate.DayOfWeek);
            endDate = startDate.AddDays(6);
            updateCurrentDateRangeString();

            reportDisposable =
                reportSubject
                    .StartWith(Unit.Default)
                    .AsObservable()
                    .Do(setLoadingState)
                    .SelectMany(_ => reportsProvider.GetProjectSummary(workspaceId, startDate, endDate))
                    .Subscribe(onReport, onError);
        }

        private void setLoadingState(Unit obj)
        {
            IsLoading = true;
            Segments.Clear();
        }

        private void onReport(ProjectSummaryReport report)
        {
            TotalTime = TimeSpan.FromSeconds(report.TotalSeconds);
            BillablePercentage = report.TotalSeconds == 0 ? null : (float?)report.BillablePercentage;
            Segments.AddRange(report.Segments);
            IsLoading = false;

            RaisePropertyChanged(nameof(Segments));
        }

        private void onError(Exception ex)
        {
            RaisePropertyChanged(nameof(Segments));
            IsLoading = false;
        }

        private void changeDateRange(DateRangeParameter dateRange)
        {
            startDate = dateRange.StartDate;
            endDate = dateRange.EndDate;
            updateCurrentDateRangeString();
            reportSubject.OnNext(Unit.Default);
        }

        private void updateCurrentDateRangeString()
            => CurrentDateRangeString = IsCurrentWeek
                ? Resources.ThisWeek
                : $"{startDate.ToString(dateFormat)} - {endDate.ToString(dateFormat)}";
    }
}
