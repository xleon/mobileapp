using MvvmCross.ViewModels;
using System.Threading.Tasks;

namespace Toggl.Core.UI.ViewModels
{
    public abstract class ViewModel<TInput, TOutput> : MvxViewModel<TInput, TOutput>
    {
        public new TaskCompletionSource<TOutput> CloseCompletionSource { get; set; }
    }

    public abstract class ViewModel : MvxViewModel
    {
    }
}
