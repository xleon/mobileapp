using System.Threading.Tasks;

namespace Toggl.Core.UI.Views
{
    public interface IView : IDialogProviderView, IPermissionHandler, IGoogleTokenProvider
    {
        Task Close();
    }
}
