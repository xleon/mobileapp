using MvvmCross.Core.ViewModels;
using MvvmCross.Platform;
using Toggl.Foundation.DataSources;

namespace Toggl.Foundation.MvvmCross.ViewModels
{
    public class BaseViewModel : MvxViewModel
    {
        private ITogglDataSource dataSource;
        protected ITogglDataSource DataSource => dataSource ?? (dataSource = Mvx.Resolve<ITogglDataSource>());
    }
}
