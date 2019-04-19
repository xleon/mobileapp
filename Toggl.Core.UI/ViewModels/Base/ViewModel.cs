using MvvmCross.ViewModels;

namespace Toggl.Core.UI.ViewModels
{
    public abstract class ViewModel<TParameter, TResult> : MvxViewModel<TParameter, TResult>
    {
    }

    public abstract class ViewModel : MvxViewModel
    {
    }
}
