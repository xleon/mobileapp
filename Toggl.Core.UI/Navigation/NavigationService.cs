using System;
using System.Threading;
using System.Threading.Tasks;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using Toggl.Core.Analytics;
using Toggl.Core.UI.ViewModels;
using Toggl.Shared;

namespace Toggl.Core.UI.Navigation
{
    public sealed class NavigationService : MvxNavigationService, INavigationService
    {
        private readonly CompositePresenter presenter;
        private readonly IAnalyticsService analyticsService;
        private readonly ViewModelLoader viewModelLocator;

        public NavigationService(
            CompositePresenter presenter,
            ViewModelLoader viewModelLocator,
            IAnalyticsService analyticsService)
            : base(null, null)
        {
            Ensure.Argument.IsNotNull(presenter, nameof(presenter));
            Ensure.Argument.IsNotNull(analyticsService, nameof(analyticsService));
            Ensure.Argument.IsNotNull(viewModelLocator, nameof(viewModelLocator));
            
            this.presenter = presenter;
            this.analyticsService = analyticsService;
            this.viewModelLocator = viewModelLocator;
        }

        public async Task<TOutput> Navigate<TViewModel, TInput, TOutput>(TInput payload)
            where TViewModel : ViewModel<TInput, TOutput>
        {
            //TODO: Pass the payload for initialization once the lifecycle is unified
            var viewModel = (TViewModel)viewModelLocator.Load(typeof(TViewModel), null, null);
            await presenter.Present(viewModel);

            analyticsService.CurrentPage.Track(typeof(TViewModel));

            viewModel.CloseCompletionSource = new TaskCompletionSource<TOutput>();
            return await viewModel.CloseCompletionSource.Task;
        }

        #region Old implementation, delete when removing MvvmCross
        public NavigationService(
            IMvxNavigationCache navigationCache,
            IMvxViewModelLoader viewModelLoader,
            IAnalyticsService analyticsService)
            : base(navigationCache, viewModelLoader)
        {
            this.analyticsService = analyticsService;
        }

        public override Task Navigate(IMvxViewModel viewModel, IMvxBundle presentationBundle = null)
        {
            analyticsService.CurrentPage.Track(viewModel.GetType());
            return base.Navigate(viewModel, presentationBundle);
        }

        public override Task Navigate<TParameter>(IMvxViewModel<TParameter> viewModel, TParameter param, IMvxBundle presentationBundle = null)
        {
            analyticsService.CurrentPage.Track(viewModel.GetType());
            return base.Navigate<TParameter>(viewModel, param, presentationBundle);
        }

        public override Task<TResult> Navigate<TResult>(IMvxViewModelResult<TResult> viewModel, IMvxBundle presentationBundle = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            analyticsService.CurrentPage.Track(viewModel.GetType());
            return base.Navigate<TResult>(viewModel, presentationBundle, cancellationToken);
        }

        public override Task<TResult> Navigate<TParameter, TResult>(IMvxViewModel<TParameter, TResult> viewModel, TParameter param, IMvxBundle presentationBundle = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            analyticsService.CurrentPage.Track(viewModel.GetType());
            return base.Navigate<TParameter, TResult>(viewModel, param, presentationBundle, cancellationToken);
        }

        public override Task Navigate(Type viewModelType, IMvxBundle presentationBundle = null)
        {
            analyticsService.CurrentPage.Track(viewModelType);
            return base.Navigate(viewModelType, presentationBundle);
        }

        public override Task Navigate<TParameter>(Type viewModelType, TParameter param, IMvxBundle presentationBundle = null)
        {
            analyticsService.CurrentPage.Track(viewModelType);
            return base.Navigate<TParameter>(viewModelType, param, presentationBundle);
        }

        public override Task<TResult> Navigate<TResult>(Type viewModelType, IMvxBundle presentationBundle = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            analyticsService.CurrentPage.Track(viewModelType);
            return base.Navigate<TResult>(viewModelType, presentationBundle, cancellationToken);
        }

        public override Task<TResult> Navigate<TParameter, TResult>(Type viewModelType, TParameter param, IMvxBundle presentationBundle = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            analyticsService.CurrentPage.Track(viewModelType);
            return base.Navigate<TParameter, TResult>(viewModelType, param, presentationBundle, cancellationToken);
        }
        #endregion
    }
}
