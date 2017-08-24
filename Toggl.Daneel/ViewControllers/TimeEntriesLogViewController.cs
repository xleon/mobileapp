using MvvmCross.Binding.BindingContext;
using MvvmCross.Binding.iOS;
using MvvmCross.iOS.Views;
using MvvmCross.Plugins.Color.iOS;
using MvvmCross.Plugins.Visibility;
using Toggl.Foundation;
using Toggl.Foundation.MvvmCross.Helper;
using Toggl.Foundation.MvvmCross.ViewModels;
using Toggl.Daneel.ViewSources;
using UIKit;
using Toggl.Daneel.Presentation.Attributes;

namespace Toggl.Daneel.ViewControllers
{
    [NestedPresentation]
    public partial class TimeEntriesLogViewController : MvxViewController<TimeEntriesLogViewModel>
    {
        public TimeEntriesLogViewController()
            : base(nameof(TimeEntriesLogViewController), null)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            //Empty state button config
            EmptyStateButton.Layer.BorderWidth = 1;
            EmptyStateButton.Layer.BorderColor = Color.TimeEntriesLog.ButtonBorder.ToNativeColor().CGColor;
            EmptyStateButton.SetTitle(Resources.TimeEntriesLogEmptyStateButton, UIControlState.Normal);

            //TableView config
            var source = new TimeEntriesLogViewSource(TimeEntriesTableView);
            TimeEntriesTableView.Source = source;

            //Converters
            var visibilityConverter = new MvxVisibilityValueConverter();
            var invertedVisibilityConverter = new MvxInvertedVisibilityValueConverter();

            var bindingSet = this.CreateBindingSet<TimeEntriesLogViewController, TimeEntriesLogViewModel>();

            //Text
            bindingSet.Bind(EmptyStateTextLabel).To(vm => vm.EmptyStateText);
            bindingSet.Bind(EmptyStateTitleLabel).To(vm => vm.EmptyStateTitle);

            //Time entries log
            bindingSet.Bind(source).To(vm => vm.TimeEntries);

            //Visibility
            bindingSet.Bind(EmptyStateView)
                      .For(v => v.BindVisibility())
                      .To(vm => vm.IsEmpty)
                      .WithConversion(visibilityConverter);

            bindingSet.Bind(EmptyStateButton)
                      .For(v => v.BindVisibility())
                      .To(vm => vm.IsWelcome)
                      .WithConversion(invertedVisibilityConverter);

            bindingSet.Bind(TimeEntriesTableView)
                      .For(v => v.BindVisibility())
                      .To(vm => vm.IsEmpty)
                      .WithConversion(invertedVisibilityConverter);

            bindingSet.Bind(EmptyStateImageView)
                      .For(v => v.BindVisibility())
                      .To($"{nameof(TimeEntriesLogViewModel.IsEmpty)}&&!{nameof(TimeEntriesLogViewModel.IsWelcome)}")
                      .WithConversion(visibilityConverter);

            bindingSet.Bind(WelcomeImageView)
                      .For(v => v.BindVisibility())
                      .To($"{nameof(TimeEntriesLogViewModel.IsEmpty)}&&{nameof(TimeEntriesLogViewModel.IsWelcome)}")
                      .WithConversion(visibilityConverter);

            //Commands
            bindingSet.Bind(source)
                      .For(s => s.SelectionChangedCommand)
                      .To(vm => vm.EditCommand);

            bindingSet.Apply();
        }
    }
}
