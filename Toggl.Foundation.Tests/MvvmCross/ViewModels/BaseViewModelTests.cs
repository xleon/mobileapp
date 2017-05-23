using MvvmCross.Test.Core;
using NSubstitute;
using Toggl.Foundation.DataSources;
using Toggl.Foundation.MvvmCross.ViewModels;
using Toggl.PrimeRadiant;
using Toggl.Ultrawave;

namespace Toggl.Foundation.Tests.MvvmCross.ViewModels
{
    public abstract class BaseViewModelTests<TViewModel> : MvxIoCSupportingTest
        where TViewModel : BaseViewModel
    {
        protected BaseViewModelTests()
        {
            Setup();

            ViewModel = Ioc.IoCConstruct<TViewModel>();
        }

        protected TViewModel ViewModel { get; }

        protected ITogglClient Api { get; } = Substitute.For<ITogglClient>();
        protected ITogglDatabase Database { get; } = Substitute.For<ITogglDatabase>();
        protected ITogglDataSource DataSource { get; } = Substitute.For<ITogglDataSource>();

        protected override void AdditionalSetup()
        {
            Ioc.RegisterSingleton(Api);
            Ioc.RegisterSingleton(Database);
            Ioc.RegisterSingleton(DataSource);
        }
     }
}
