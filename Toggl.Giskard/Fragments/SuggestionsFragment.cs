using System.Collections.Specialized;
using System.Linq;
using Android.OS;
using Android.Views;
using Android.Widget;
using MvvmCross.Binding.Droid.BindingContext;
using MvvmCross.Droid.Support.V4;
using MvvmCross.Droid.Views.Attributes;
using Toggl.Foundation.MvvmCross.ViewModels;
using Toggl.Giskard.Activities;
using Toggl.Giskard.Extensions;

namespace Toggl.Giskard.Fragments
{
    [MvxFragmentPresentation(typeof(MainViewModel), Resource.Id.MainSuggestionsContainer)]
    public sealed class SuggestionsFragment : MvxFragment<SuggestionsViewModel>
    {
        private int containerHeightInPixels;

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            var view = this.BindingInflate(Resource.Layout.SuggestionsFragment, null);

            containerHeightInPixels = (int)130.DpToPixels(Activity);
            ViewModel.Suggestions.CollectionChanged += onCollectionChanged;
            calculateContainerHeight();
            return view;
        }

        private void onCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            calculateContainerHeight();
        }

        private void calculateContainerHeight()
        {
            var mainActivity = (MainActivity)Activity;
            var height = ViewModel.Suggestions.Any() ? containerHeightInPixels : 0;
            var newParams = new RelativeLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, height);
            mainActivity.SuggestionsContainer.LayoutParameters = newParams;
        }
    }
}
