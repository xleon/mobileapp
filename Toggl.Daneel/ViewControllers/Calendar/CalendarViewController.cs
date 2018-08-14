using MvvmCross;
using MvvmCross.Platforms.Ios.Presenters.Attributes;
using Toggl.Daneel.Extensions;
using Toggl.Daneel.Views.Calendar;
using Toggl.Daneel.ViewSources;
using Toggl.Foundation;
using Toggl.Foundation.MvvmCross.ViewModels;
using Toggl.Foundation.MvvmCross.ViewModels.Calendar;

namespace Toggl.Daneel.ViewControllers
{
    [MvxChildPresentation]
    public sealed partial class CalendarViewController : ReactiveViewController<CalendarViewModel>
    {
        private CalendarCollectionViewLayout layout;
        private CalendarCollectionViewSource dataSource;

        public CalendarViewController() : base(nameof(CalendarViewController))
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            this.Bind(ViewModel.ShouldShowOnboarding, OnboardingView.BindIsVisibleWithFade());
            this.Bind(GetStartedButton.Tapped(), ViewModel.GetStartedAction);

            var timeService = Mvx.Resolve<ITimeService>();

            dataSource = new CalendarCollectionViewSource(CalendarCollectionView, ViewModel.CalendarItems);
            layout = new CalendarCollectionViewLayout(timeService, dataSource);

            CalendarCollectionView.SetCollectionViewLayout(layout, false);
            CalendarCollectionView.DataSource = dataSource;
        }
    }
}
