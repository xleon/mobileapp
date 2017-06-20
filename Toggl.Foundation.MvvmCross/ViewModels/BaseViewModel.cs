using System;
using System.Threading.Tasks;
using MvvmCross.Core.ViewModels;
using MvvmCross.Platform;
using Toggl.Foundation.DataSources;

namespace Toggl.Foundation.MvvmCross.ViewModels
{
    public class BaseViewModel : MvxViewModel
    {
        protected ITogglDataSource DataSource => Mvx.Resolve<ITogglDataSource>();
    }

    public abstract class BaseViewModel<TParameter> : MvxViewModel<TParameter>
        where TParameter : class
    {
        protected ITogglDataSource DataSource => Mvx.Resolve<ITogglDataSource>();
    }
}
