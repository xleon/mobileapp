using System.Threading.Tasks;
using Toggl.Core.UI.ViewModels;

namespace Toggl.Core.Tests.TestExtensions
{
    internal static class ViewModelExtensions
    {
        public static Task<TOutput> ReturnedValue<TInput, TOutput>(this ViewModel<TInput, TOutput> viewModel)
            => viewModel.CloseCompletionSource.Task;
    }
}
