using System.Threading.Tasks;
using Toggl.Core.UI.Navigation;

namespace Toggl.Droid.Presentation
{
    public abstract class AndroidPresenter : IPresenter
    {
        public abstract bool CanPresent<T>(NavigationInfo<T> navigationInfo);

        public abstract Task<TOutput> Present<TInput, TOutput>(NavigationInfo<TInput> navigationInfo);
    }
}
