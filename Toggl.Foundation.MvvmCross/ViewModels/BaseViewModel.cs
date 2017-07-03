using MvvmCross.Core.ViewModels;
using PropertyChanged;

namespace Toggl.Foundation.MvvmCross.ViewModels
{
    [ImplementPropertyChanged]
    public abstract class BaseViewModel : MvxViewModel
    {
        public string Title { get; set; }
    }

    [ImplementPropertyChanged]
    public abstract class BaseViewModel<TParameter> : MvxViewModel<TParameter>
        where TParameter : class
    {
        public string Title { get; set; }
    }
}
