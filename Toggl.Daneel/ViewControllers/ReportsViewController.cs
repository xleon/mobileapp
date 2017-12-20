using MvvmCross.Binding.BindingContext;
using MvvmCross.iOS.Views;
using MvvmCross.iOS.Views.Presenters.Attributes;
using Toggl.Daneel.ViewSources;
using Toggl.Foundation.MvvmCross.ViewModels;
using static Toggl.Daneel.Extensions.LayoutConstraintExtensions;

namespace Toggl.Daneel.ViewControllers
{
    [MvxChildPresentation]
    public sealed partial class ReportsViewController : MvxViewController<ReportsViewModel>
    {
        public ReportsViewController() : base(nameof(ReportsViewController), null)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            prepareViews();

            var source = new ReportsTableViewSource(ReportsTableView);
            ReportsTableView.Source = source;

            var bindingSet = this.CreateBindingSet<ReportsViewController, ReportsViewModel>();

            bindingSet.Bind(this)
                      .For(v => v.Title)
                      .To(vm => vm.CurrentDateRangeString);

            bindingSet.Bind(source).To(vm => vm.Segments);

            bindingSet.Bind(source).For(v => v.ViewModel).To(vm => vm);

            bindingSet.Apply();
        }

        private void prepareViews()
        {
            TopConstraint.AdaptForIos10(NavigationController.NavigationBar);
        }
    }
}

