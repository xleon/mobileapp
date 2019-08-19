using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using System;
using System.Linq;
using System.Reactive.Linq;
using Toggl.Core.UI.Extensions;
using Toggl.Core.UI.ViewModels;
using Toggl.Droid.Adapters;
using Toggl.Droid.Extensions.Reactive;
using Toggl.Droid.Presentation;
using Toggl.Shared.Extensions;

namespace Toggl.Droid.Activities
{
    [Activity(Theme = "@style/Theme.Splash",
        ScreenOrientation = ScreenOrientation.Portrait,
        ConfigurationChanges = ConfigChanges.Orientation | ConfigChanges.ScreenSize)]
    public partial class SelectTagsActivity : ReactiveActivity<SelectTagsViewModel>
    {
        public SelectTagsActivity() : base(
            Resource.Layout.SelectTagsActivity,
            Resource.Style.AppTheme_Light_WhiteBackground,
            Transitions.SlideInFromBottom)
        { }

        public SelectTagsActivity(IntPtr javaReference, JniHandleOwnership transfer)
            : base(javaReference, transfer)
        {
        }

        protected override void InitializeBindings()
        {
            ViewModel.Tags
                .Select(tags => tags.ToList())
                .Subscribe(selectTagsRecyclerAdapter.Rx().Items())
                .DisposedBy(DisposeBag);

            ViewModel.FilterText
                .Select(text => text == string.Empty)
                .DistinctUntilChanged()
                .Subscribe(saveButton.Rx().IsVisible())
                .DisposedBy(DisposeBag);

            ViewModel.FilterText
                .Select(text => text == string.Empty)
                .DistinctUntilChanged()
                .Invert()
                .Subscribe(clearIcon.Rx().IsVisible())
                .DisposedBy(DisposeBag);

            backIcon.Rx().Tap()
                .Subscribe(ViewModel.CloseWithDefaultResult)
                .DisposedBy(DisposeBag);

            clearIcon.Rx().Tap()
                .SelectValue(string.Empty)
                .Subscribe(textField.Rx().TextObserver())
                .DisposedBy(DisposeBag);

            saveButton.Rx()
                .BindAction(ViewModel.Save)
                .DisposedBy(DisposeBag);

            textField.Rx().Text()
                .Subscribe(ViewModel.FilterText)
                .DisposedBy(DisposeBag);

            selectTagsRecyclerAdapter.ItemTapObservable
                .Subscribe(ViewModel.SelectTag.Inputs)
                .DisposedBy(DisposeBag);
        }
    }
}
