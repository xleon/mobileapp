using MvvmCross.Core;
using MvvmCross.Core.Platform;
using MvvmCross.Platform.Core;
using MvvmCross.Platform.IoC;
using MvvmCross.Platform.Platform;
using MvvmCross.Test.Core;
using NSubstitute;
using Toggl.Foundation.DataSources;
using Toggl.Foundation.MvvmCross.ViewModels;
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
        }
    }

    public abstract class BaseViewModelTests<TViewModel> : BaseViewModelTests
        where TViewModel : BaseViewModel
    {
        protected IMvxIoCProvider Ioc { get; private set; }

        protected BaseViewModelTests()
        {
            Setup();

            ViewModel = Ioc.IoCConstruct<TViewModel>();
        }

        protected TViewModel ViewModel { get; }

        protected ITogglApi Api { get; } = Substitute.For<ITogglApi>();
        protected ITogglDatabase Database { get; } = Substitute.For<ITogglDatabase>();
        protected ITogglDataSource DataSource { get; } = Substitute.For<ITogglDataSource>();

        private void Setup()
        {
            Ioc = MvxSingleton<IMvxIoCProvider>.Instance;

            AdditionalSetup();
        }

        protected virtual void AdditionalSetup()
        {
            Ioc.RegisterSingleton(Api);
            Ioc.RegisterSingleton(Database);
            Ioc.RegisterSingleton(DataSource);
        }
    }
}
