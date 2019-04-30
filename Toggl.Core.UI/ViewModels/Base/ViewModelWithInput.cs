using System.Reactive;

namespace Toggl.Core.UI.ViewModels
{
    public abstract class ViewModelWithInput<TInput> : ViewModel<TInput, Unit>
    {
    }
}
