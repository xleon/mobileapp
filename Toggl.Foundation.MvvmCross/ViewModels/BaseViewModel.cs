using MvvmCross.Core.ViewModels;

namespace Toggl.Foundation.MvvmCross.ViewModels
{
    public class BaseViewModel : MvxViewModel
    {
    }

    public abstract class BaseViewModel<TParameter> : MvxViewModel<TParameter>
        where TParameter : class
    {
    }
}
