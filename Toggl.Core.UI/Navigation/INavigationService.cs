using MvvmCross.Navigation;
using System.Reactive;
using System.Threading.Tasks;
using Toggl.Core;
using Toggl.Core.UI.ViewModels;
using Toggl.Core.UI.ViewModels.Calendar;
using Toggl.Core.UI.ViewModels.Reports;

//TODO: Remove when implementing deep linking
[assembly: MvxNavigation(typeof(MainViewModel), ApplicationUrls.Main.Regex)]
[assembly: MvxNavigation(typeof(ReportsViewModel), ApplicationUrls.Reports)]
[assembly: MvxNavigation(typeof(CalendarViewModel), ApplicationUrls.Calendar.Regex)]
[assembly: MvxNavigation(typeof(StartTimeEntryViewModel), ApplicationUrls.StartTimeEntry)]

namespace Toggl.Core.UI.Navigation
{
    public interface INavigationService
    {
        Task<TOutput> Navigate<TViewModel, TInput, TOutput>(TInput payload)
               where TViewModel : ViewModel<TInput, TOutput>;
    }

    public static class NavigationServiceExtensions
    {
        public static Task Navigate<TViewModel>(this INavigationService navigationService)
            where TViewModel : ViewModel<Unit, Unit>
            => navigationService.Navigate<TViewModel, Unit, Unit>(Unit.Default);

        public static Task<TOutput> Navigate<TViewModel, TOutput>(this INavigationService navigationService)
            where TViewModel : ViewModel<Unit, TOutput>
            => navigationService.Navigate<TViewModel, Unit, TOutput>(Unit.Default);

        public static Task Navigate<TViewModel, TInput>(this INavigationService navigationService, TInput payload)
            where TViewModel : ViewModel<TInput, Unit>
            => navigationService.Navigate<TViewModel, TInput, Unit>(payload);
    }
}
