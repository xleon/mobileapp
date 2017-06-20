using MvvmCross.Core;
using MvvmCross.Core.Navigation;
using MvvmCross.Core.Platform;
using MvvmCross.Core.ViewModels;
using MvvmCross.Platform.Core;
using MvvmCross.Platform.IoC;
using MvvmCross.Platform.Platform;
using MvvmCross.Test.Core;
using NSubstitute;
using Toggl.Foundation.DataSources;
using Toggl.PrimeRadiant;
using Toggl.Ultrawave;

namespace Toggl.Foundation.Tests.MvvmCross.ViewModels
{
    public abstract class BaseViewModelTests
    {
        static BaseViewModelTests()
        {
            if (MvxSingleton<IMvxIoCProvider>.Instance != null) return;

            MvxSingletonCache.Initialize();
            MvxSimpleIoCContainer.Initialize();

            MvxSingleton<IMvxIoCProvider>.Instance.RegisterSingleton(MvxSingleton<IMvxIoCProvider>.Instance);
            MvxSingleton<IMvxIoCProvider>.Instance.RegisterSingleton<IMvxTrace>(new TestTrace());
            MvxSingleton<IMvxIoCProvider>.Instance.RegisterSingleton<IMvxSettings>(new MvxSettings());

            MvxTrace.Initialize();

            MvxSingleton<IMvxIoCProvider>.Instance.RegisterSingleton(Api);
            MvxSingleton<IMvxIoCProvider>.Instance.RegisterSingleton(Database);
            MvxSingleton<IMvxIoCProvider>.Instance.RegisterSingleton(DataSource);
            MvxSingleton<IMvxIoCProvider>.Instance.RegisterSingleton(NavigationService);
        }

        protected static ITogglApi Api { get; } = Substitute.For<ITogglApi>();
        protected static ITogglDatabase Database { get; } = Substitute.For<ITogglDatabase>();
        protected static ITogglDataSource DataSource { get; } = Substitute.For<ITogglDataSource>();
        protected static IMvxNavigationService NavigationService { get; } = Substitute.For<IMvxNavigationService>();
    }

    public abstract class BaseViewModelTests<TViewModel> : BaseViewModelTests
        where TViewModel : MvxViewModel
    {
        protected IMvxIoCProvider Ioc { get; private set; }

        protected BaseViewModelTests()
        {
            Setup();

            ViewModel = Ioc.IoCConstruct<TViewModel>();
        }

        protected TViewModel ViewModel { get; }

        private void Setup()
        {
            Ioc = MvxSingleton<IMvxIoCProvider>.Instance;

            AdditionalSetup();
        }

        protected virtual void AdditionalSetup()
        {
        }
    }
}
