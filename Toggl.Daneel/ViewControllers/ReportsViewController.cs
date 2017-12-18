using MvvmCross.Binding.BindingContext;
using MvvmCross.Binding.iOS;
using MvvmCross.iOS.Views;
using MvvmCross.iOS.Views.Presenters.Attributes;
using MvvmCross.Plugins.Color.iOS;
using Toggl.Daneel.Converters;
using Toggl.Foundation.MvvmCross.Converters;
using Toggl.Foundation.MvvmCross.Helper;
using Toggl.Foundation.MvvmCross.ViewModels;
using UIKit;
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

            var bindingSet = this.CreateBindingSet<ReportsViewController, ReportsViewModel>();

            //Text
            bindingSet.Bind(this)
                      .For(v => v.Title)
                      .To(vm => vm.CurrentDateRangeString);

            bindingSet.Bind(BillablePercentageLabel)
                      .For(v => v.AttributedText)
                      .To(vm => vm.BillablePercentage)
                      .WithConversion(new ReportPercentageLabelValueConverter());

            bindingSet.Bind(TotalDurationLabel)
                      .For(v => v.AttributedText)
                      .To(vm => vm.TotalTime)
                      .WithConversion(new TimeSpanReportLabelValueConverter());

            //Pretty stuff
            bindingSet.Bind(PieChartView)
                      .For(v => v.Segments)
                      .To(vm => vm.Segments);

            bindingSet.Bind(BillablePercentageView)
                      .For(v => v.Percentage)
                      .To(vm => vm.BillablePercentage);

            bindingSet.Bind(TotalDurationGraph)
                      .For(v => v.TintColor)
                      .To(vm => vm.TotalTimeIsZero)
                      .WithConversion(new BoolToConstantValueConverter<UIColor>(
                          Color.Reports.Disabled.ToNativeColor(),
                          Color.Reports.TotalTimeActivated.ToNativeColor()
                      ));

            //Visibility
            bindingSet.Bind(EmptyStateView)
                      .For(v => v.BindVisible())
                      .To(vm => vm.ShowEmptyState);

            bindingSet.Apply();
        }

        private void prepareViews()
        {
            TopConstraint.AdaptForIos10(NavigationController.NavigationBar);

            var templateImage = TotalDurationGraph.Image.ImageWithRenderingMode(UIImageRenderingMode.AlwaysTemplate);
            TotalDurationGraph.Image = templateImage; 
        }
    }
}

