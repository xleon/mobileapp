using MvvmCross.Core.ViewModels;

namespace Toggl.Foundation.MvvmCross.ViewModels
{
    public abstract class BaseViewModel : MvxViewModel
    {
        public string Title { get; set; }
    }

    public abstract class BaseViewModel<TParameter> : MvxViewModel<TParameter>
        where TParameter : class
    {
        public string Title { get; set; }
    }
}
