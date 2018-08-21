using System.Threading.Tasks;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;

namespace Toggl.Foundation.MvvmCross.Services
{
    public interface IForkingNavigationService : IMvxNavigationService
    {
        Task ForkNavigate<TDaneelViewModel, TGiskardViewModel>()
            where TDaneelViewModel : IMvxViewModel
            where TGiskardViewModel : IMvxViewModel;
    }
}
