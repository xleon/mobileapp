using System;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using Toggl.Core.UI.ViewModels;
using Toggl.Core.UI.ViewModels.Selectable;
using Toggl.Core.Services;
using Toggl.Core.Tests.Generators;
using Toggl.Shared;
using Xunit;
using Toggl.Core.UI.Navigation;
using Toggl.Core.Tests.TestExtensions;

namespace Toggl.Core.Tests.UI.ViewModels
{
    public sealed class SelectDateFormatViewModelTests
    {
        public abstract class SelectDateFormatViewModelTest : BaseViewModelTests<SelectDateFormatViewModel, DateFormat, DateFormat>
        {
            protected override SelectDateFormatViewModel CreateViewModel()
                => new SelectDateFormatViewModel(NavigationService, RxActionFactory);
        }

        public sealed class TheConstructor
        {
            [Theory, LogIfTooSlow]
            [ConstructorData]
            public void ThrowsIfTheArgumentIsNull(bool useNavigationService, bool useRxActionFactory)
            {
                var navigationService = useNavigationService ? Substitute.For<INavigationService>() : null;
                var rxActionFactory = useRxActionFactory ? Substitute.For<IRxActionFactory>() : null;

                Action tryingToConstructWithEmptyParameter =
                    () => new SelectDateFormatViewModel(navigationService, rxActionFactory);

                tryingToConstructWithEmptyParameter.Should().Throw<ArgumentNullException>();
            }
        }

        public sealed class ThePrepareMethod : SelectDateFormatViewModelTest
        {
            [Fact, LogIfTooSlow]
            public void MarksTheSelectedDateFormatAsSelected()
            {
                var selectedDateFormat = ViewModel.DateTimeFormats[0];

                ViewModel.Initialize(selectedDateFormat.DateFormat);

                selectedDateFormat.Selected.Should().BeTrue();
            }
        }

        public sealed class TheCloseCommand : SelectDateFormatViewModelTest
        {
            [Fact, LogIfTooSlow]
            public async Task ClosesTheViewModelPassingTheDefaultResult()
            {
                var defaultResult = DateFormat.FromLocalizedDateFormat("YYYY.MM.DD");
                await ViewModel.Initialize(defaultResult);

                ViewModel.Close.Execute();
                TestScheduler.Start();

                (await ViewModel.ReturnedValue()).Should().Be(defaultResult);
            }
        }

        public sealed class TheSelectFormatCommand : SelectDateFormatViewModelTest
        {
            [Fact, LogIfTooSlow]
            public async Task ClosesTheViewModelPassingTheSelectedResult()
            {
                var selectedDateFormat = DateFormat.FromLocalizedDateFormat("DD.MM.YYYY");
                var selectableDateFormatViewModel = new SelectableDateFormatViewModel(selectedDateFormat, false);

                ViewModel.SelectDateFormat.Execute(selectableDateFormatViewModel);
                TestScheduler.Start();

                (await ViewModel.ReturnedValue()).Should().Be(selectedDateFormat);    
            }
        }
    }
}
