using System.Threading.Tasks;
using Toggl.Core.UI.Navigation;
using UIKit;

namespace Toggl.iOS.Presentation
{
    public abstract class IosPresenter : IPresenter
    {
        protected UIWindow Window { get; }

        protected AppDelegate AppDelegate { get; }

        public IosPresenter(UIWindow window, AppDelegate appDelegate)
        {
            Window = window;
            AppDelegate = appDelegate;
        }

        public abstract bool CanPresent<TInput>(NavigationInfo<TInput> navigationInfo);

        public abstract Task<TOutput> Present<TInput, TOutput>(NavigationInfo<TInput> navigationInfo);
    }
}
