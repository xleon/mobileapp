using System.Threading.Tasks;

namespace Toggl.iOS.ViewControllers
{
    public interface IDismissableViewController
    {
        Task<bool> Dismiss();
    }
}
