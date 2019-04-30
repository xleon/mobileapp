using System.Reactive;
using System.Threading.Tasks;

namespace Toggl.Core.UI.ViewModels
{
    public abstract class ViewModelWithOutput<TOutput> : ViewModel<Unit, TOutput>
    {
        public virtual Task Initialize()
            => Task.CompletedTask;

        public sealed override Task Initialize(Unit payload)
            => Initialize();
    }
}
