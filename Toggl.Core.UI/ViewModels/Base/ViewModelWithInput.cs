using MvvmCross.ViewModels;
using Toggl.Core.UI.Views;

namespace Toggl.Core.UI.ViewModels
{
    public abstract class ViewModelWithInput<TParameter> : MvxViewModel<TParameter>, IViewModel
    {
        public IView View { get; set; }

        public void AttachView(IView viewToAttach)
        {
            View = viewToAttach;
        }

        public void DetachView()
        {
            View = null;
        }
    }
}
