using MvvmCross.Test.Core;
using NSubstitute;
using Toggl.Foundation.DataSources;
using Toggl.Foundation.MvvmCross.ViewModels;

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

        protected ITogglDataSource DataSource { get; } = Substitute.For<ITogglDataSource>();

        protected override void AdditionalSetup()
            => Ioc.RegisterSingleton(DataSource);
     }
}
