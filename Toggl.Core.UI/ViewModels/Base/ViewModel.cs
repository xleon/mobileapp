using System.Threading.Tasks;
using MvvmCross.ViewModels;
using Toggl.Core.UI.Views;

namespace Toggl.Core.UI.ViewModels
{
    public abstract class ViewModel<TInput, TOutput> : MvxViewModel<TInput, TOutput>, IViewModel
    {
        public new TaskCompletionSource<TOutput> CloseCompletionSource { get; set; }
        
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

    public abstract class ViewModel : MvxViewModel, IViewModel
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
