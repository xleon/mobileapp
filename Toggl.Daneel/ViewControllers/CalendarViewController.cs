using MvvmCross.Platforms.Ios.Presenters.Attributes;
using Toggl.Foundation.MvvmCross.ViewModels;

namespace Toggl.Daneel.ViewControllers
{
    [MvxChildPresentation]
    public sealed partial class CalendarViewController
        : ReactiveViewController<CalendarViewModel>
    {
        public CalendarViewController() : base(null)
        {
        }
    }
}

