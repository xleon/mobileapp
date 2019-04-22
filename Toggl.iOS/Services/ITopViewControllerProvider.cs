using UIKit;

namespace Toggl.iOS.Services
{
    public interface ITopViewControllerProvider
    {
        UIViewController TopViewController { get; }
    }
}
