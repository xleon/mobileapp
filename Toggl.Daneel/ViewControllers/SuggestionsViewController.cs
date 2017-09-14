using MvvmCross.Binding.BindingContext;
using MvvmCross.Binding.iOS;
using MvvmCross.iOS.Views;
using MvvmCross.Plugins.Visibility;
using Toggl.Daneel.Presentation.Attributes;
using Toggl.Daneel.ViewSources;
using Toggl.Foundation;
using Toggl.Foundation.MvvmCross.ViewModels;

namespace Toggl.Daneel.ViewControllers
{
    [NestedPresentation]
    public partial class SuggestionsViewController : MvxViewController<SuggestionsViewModel>
    {
        public SuggestionsViewController() 
            : base(nameof(SuggestionsViewController), null)
        {
        }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            WelcomeTextLabel.Text = Resources.SuggestionsEmptyStateText;
            WelcomeTitleLabel.Text = Resources.SuggestionsEmptyStateTitle;

            //TableView setup
            var source = new SuggestionsTableViewSource(SuggestionsTableView);
            SuggestionsTableView.Source = source;

            var bindingSet = this.CreateBindingSet<SuggestionsViewController, SuggestionsViewModel>();

            //Suggestions table view
            bindingSet.Bind(source).To(vm => vm.Suggestions);
            bindingSet.Bind(WelcomeView)
                      .For(v => v.BindVisibility())
                      .To(vm => vm.IsEmpty)
                      .WithConversion(new MvxVisibilityValueConverter());
            
            bindingSet.Apply();
        }
    }
}
