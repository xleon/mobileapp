using System.Reactive;
using System.Threading.Tasks;

namespace Toggl.Core.UI.ViewModels
{
    public abstract class ViewModelWithInput<TInput> : ViewModel<TInput, Unit>
    {
        public Task Finish()
            => Finish(Unit.Default);
    }
}
