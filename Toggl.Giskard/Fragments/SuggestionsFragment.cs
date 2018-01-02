using Android.OS;
using Android.Views;
using MvvmCross.Binding.Droid.BindingContext;
using MvvmCross.Droid.Support.V4;
using MvvmCross.Droid.Views.Attributes;
using Toggl.Foundation.MvvmCross.ViewModels;

namespace Toggl.Giskard.Fragments
{
    [MvxFragmentPresentation(typeof(MainViewModel), Resource.Id.MainSuggestionsContainer, ActivityHostViewModelType = typeof(MainViewModel))]
    public sealed class SuggestionsFragment : MvxFragment
    {
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            var view = this.BindingInflate(Resource.Layout.SuggestionsFragment, null);
            return view;
        }
    }
}
