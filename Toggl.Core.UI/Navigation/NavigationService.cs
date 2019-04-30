using System.Threading.Tasks;
using Toggl.Core.Analytics;
using Toggl.Core.UI.ViewModels;
using Toggl.Shared;

namespace Toggl.Core.UI.Navigation
{
    public sealed class NavigationService : INavigationService
    {
        private readonly CompositePresenter presenter;
        private readonly IAnalyticsService analyticsService;
        private readonly ViewModelLoader viewModelLocator;

        public NavigationService(
            CompositePresenter presenter,
            ViewModelLoader viewModelLocator,
            IAnalyticsService analyticsService)
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
            var viewModel = await viewModelLocator.Load<TInput, TOutput>(typeof(TViewModel), payload);
            await presenter.Present(viewModel);

            analyticsService.CurrentPage.Track(typeof(TViewModel));

            viewModel.CloseCompletionSource = new TaskCompletionSource<TOutput>();
            return await viewModel.CloseCompletionSource.Task;
        }
    }
}
