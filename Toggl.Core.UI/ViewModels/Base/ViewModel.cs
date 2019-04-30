using System.Reactive;
using System.Threading.Tasks;
using Toggl.Core.UI.Views;

namespace Toggl.Core.UI.ViewModels
{
    public abstract class ViewModel<TInput, TOutput> : IViewModel
    {
        public IView View { get; set; }
        public TaskCompletionSource<TOutput> CloseCompletionSource { get; set; }

        public virtual Task Initialize(TInput payload)
            => Task.CompletedTask;

        public void AttachView(IView viewToAttach)
        {
            View = viewToAttach;
        }

        public void DetachView()
        {
            View = null;
        }

        public virtual void ViewAppeared()
        {
        }

        public virtual void ViewAppearing()
        {
        }

        public virtual void ViewDisappearing()
        {
        }

        public virtual void ViewDisappeared()
        {
        }

        public virtual void ViewDestroyed()
        {
        }
    }

    public abstract class ViewModel : ViewModel<Unit, Unit>
    {
        public virtual Task Initialize()
            => Task.CompletedTask;

        public sealed override Task Initialize(Unit payload)
            => Initialize();
    }
}
