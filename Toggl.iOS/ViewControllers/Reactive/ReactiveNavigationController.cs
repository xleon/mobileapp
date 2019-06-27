using System.Linq;
using Toggl.Core.UI.Views;
using UIKit;

namespace Toggl.iOS.ViewControllers
{
    public class ReactiveNavigationController : UINavigationController
    {
        public ReactiveNavigationController(UIViewController rootViewController) : base(rootViewController)
        {
        }

        public override UIViewController PopViewController(bool animated)
        {
            var viewControllerToPop = ViewControllers.Last();
            if (viewControllerToPop is IReactiveViewController reactiveViewController)
            {
                reactiveViewController.DismissFromNavigationController();
            }

            return base.PopViewController(animated);
        }
    }
}