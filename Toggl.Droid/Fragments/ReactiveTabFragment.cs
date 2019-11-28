using Android.OS;
using Android.Runtime;
using Android.Views;
using System;
using System.Reactive.Disposables;
using AndroidX.AppCompat.Widget;
using AndroidX.Fragment.App;
using Toggl.Core.UI.Navigation;
using Toggl.Core.UI.ViewModels;
using Toggl.Core.UI.Views;
using Toggl.Droid.Extensions;

namespace Toggl.Droid.Fragments
{
    public abstract partial class ReactiveTabFragment<TViewModel> : Fragment, IView, IMenuItemOnMenuItemClickListener 
        where TViewModel : class, IViewModel
    {
        private readonly Lazy<TViewModel> lazyViewModel;
        protected CompositeDisposable DisposeBag = new CompositeDisposable();

        private bool fullyInitialized;

        public TViewModel ViewModel { get; set; }

        protected abstract int LayoutId { get; }
        protected abstract View LoadingPlaceholderView { get; set; }
        protected abstract void InitializeViews(View view);
        protected abstract void InitializationFinished();

        protected ReactiveTabFragment(MainTabBarViewModel tabBarViewModel)
        {
            lazyViewModel = tabBarViewModel.GetViewModel<TViewModel>();
        }

        protected ReactiveTabFragment(IntPtr javaReference, JniHandleOwnership transfer)
            : base(javaReference, transfer)
        {
        }

        public sealed override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            var view = inflater.Inflate(Resource.Layout.MainFragment, container, false);

            InitializeViews(view);
            SetupToolbar(view);

            LoadingPlaceholderView.Visibility = ViewStates.Visible;
            Task.Run(async () =>
            {
                ViewModel = lazyViewModel.Value;
                await ViewModel.Initialize();
                ViewModel?.ViewAppearing();

                LoadingPlaceholderView.Post(() =>
                {
                    InitializationFinished();
                    ViewModel?.ViewAppeared();
                    fullyInitialized = true;
                    LoadingPlaceholderView.Visibility = ViewStates.Gone;
                });
            });

            return view;
        }

        public override void OnViewCreated(View view, Bundle savedInstanceState)
        {
            base.OnViewCreated(view, savedInstanceState);
            ViewModel?.AttachView(this);
        }

        public override void OnStart()
        {
            base.OnStart();
            if (!fullyInitialized) return;
            ViewModel?.ViewAppearing();
        }

        public override void OnResume()
        {
            base.OnResume();

            if (IsHidden && !fullyInitialized) return;

            ViewModel?.ViewAppeared();
        }

        public override void OnPause()
        {
            base.OnPause();
            if (!fullyInitialized) return;
            ViewModel?.ViewDisappearing();
        }

        public override void OnStop()
        {
            base.OnStop();
            if (!fullyInitialized) return;
            ViewModel?.ViewDisappeared();
        }

        public override void OnDestroyView()
        {
            ViewModel?.DetachView();
            base.OnDestroyView();
            DisposeBag.Dispose();
            DisposeBag = new CompositeDisposable();
        }

        public override void OnHiddenChanged(bool hidden)
        {
            base.OnHiddenChanged(hidden);
            if (!fullyInitialized) return;

            if (hidden)
                ViewModel?.ViewDisappeared();
            else
                ViewModel?.ViewAppeared();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (!disposing) return;
            DisposeBag?.Dispose();
        }

        public void SetupToolbar(View fragmentView)
        {
            var toolbar = fragmentView.FindViewById<Toolbar>(Resource.Id.Toolbar);
            toolbar.InflateMenu(Resource.Menu.SettingsMenu);
            var saveMenuItem = toolbar.Menu.FindItem(Resource.Id.Settings);
            saveMenuItem.SetTitle(Shared.Resources.Settings);
            saveMenuItem.SetOnMenuItemClickListener(this);
        }

        public void Close()
        {
        }

        public IObservable<string> GetGoogleToken()
        {
            if (!(Activity is IGoogleTokenProvider tokenProvider))
                throw new InvalidOperationException();

            return tokenProvider.GetGoogleToken();
        }

        public bool OnMenuItemClick(IMenuItem item)
        {
            AndroidDependencyContainer
                .Instance
                .NavigationService
                .Navigate<SettingsViewModel>(this);

            return true;
        }
    }
}
