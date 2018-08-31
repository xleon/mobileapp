using MvvmCross;
using MvvmCross.Platforms.Ios.Presenters.Attributes;
using Toggl.Daneel.Extensions;
using Toggl.Daneel.Views.Calendar;
using Toggl.Daneel.ViewSources;
using Toggl.Foundation;
using Toggl.Foundation.MvvmCross.ViewModels;
using Toggl.Foundation.MvvmCross.ViewModels.Calendar;
using UIKit;

namespace Toggl.Daneel.ViewControllers
{
    [MvxChildPresentation]
    public sealed partial class CalendarViewController : ReactiveViewController<CalendarViewModel>
    {
        private readonly UIImageView titleImage = new UIImageView(UIImage.FromBundle("togglLogo"));

        private CalendarCollectionViewLayout layout;
        private CalendarCollectionViewSource dataSource;
        private CalendarCollectionViewCreateFromSpanHelper createFromSpanHelper;
        private CalendarCollectionViewEditItemHelper editItemHelper;

        public CalendarViewController() : base(nameof(CalendarViewController))
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            this.Bind(ViewModel.ShouldShowOnboarding, OnboardingView.BindIsVisibleWithFade());
            this.Bind(GetStartedButton.Tapped(), ViewModel.GetStartedAction);

            var timeService = Mvx.Resolve<ITimeService>();

            dataSource = new CalendarCollectionViewSource(
                CalendarCollectionView,
                ViewModel.Date,
                ViewModel.TimeOfDayFormat,
                ViewModel.CalendarItems);

            layout = new CalendarCollectionViewLayout(timeService, dataSource);

            createFromSpanHelper = new CalendarCollectionViewCreateFromSpanHelper(CalendarCollectionView, dataSource, layout);
            editItemHelper = new CalendarCollectionViewEditItemHelper(CalendarCollectionView, dataSource, layout);

            CalendarCollectionView.SetCollectionViewLayout(layout, false);
            CalendarCollectionView.Delegate = dataSource;
            CalendarCollectionView.DataSource = dataSource;
            CalendarCollectionView.ContentInset = new UIEdgeInsets(20, 0, 20, 0);

            this.Bind(dataSource.ItemTapped, ViewModel.OnItemTapped);
            this.Bind(createFromSpanHelper.CreateFromSpan, ViewModel.OnDurationSelected);
            this.Bind(editItemHelper.EditCalendarItem, ViewModel.OnUpdateTimeEntry);
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            NavigationItem.TitleView = titleImage;
        }
    }
}
