using MvvmCross.Binding.BindingContext;
using MvvmCross.iOS.Views;
using Toggl.Daneel.Presentation.Attributes;
using Toggl.Daneel.ViewSources;
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

            //TableView setup
            var source = new SuggestionsTableViewSource(SuggestionsTableView);
            SuggestionsTableView.Source = source;

            var bindingSet = this.CreateBindingSet<SuggestionsViewController, SuggestionsViewModel>();

            //Suggestions table view
            bindingSet.Bind(source).To(vm => vm.Suggestions);

            bindingSet.Apply();
        }
    }
}
