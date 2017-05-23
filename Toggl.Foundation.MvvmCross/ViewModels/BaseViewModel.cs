using MvvmCross.Core.ViewModels;
using MvvmCross.Platform;
using Toggl.Foundation.DataSources;

namespace Toggl.Foundation.MvvmCross.ViewModels
{
    public class BaseViewModel : MvxViewModel
    {
        protected ITogglDataSource DataSource => Mvx.Resolve<ITogglDataSource>();
    }
}
