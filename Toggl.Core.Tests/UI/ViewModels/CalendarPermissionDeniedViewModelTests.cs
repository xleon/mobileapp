﻿using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NSubstitute;
using Toggl.Core.UI.ViewModels.Calendar;
using Toggl.Core.Tests.Generators;
using Xunit;

namespace Toggl.Core.Tests.UI.ViewModels
{
    public sealed class CalendarPermissionDeniedViewModelTests
    {
        public abstract class CalendarPermissionDeniedViewModelTest
            : BaseViewModelTests<CalendarPermissionDeniedViewModel>
        {
            protected override CalendarPermissionDeniedViewModel CreateViewModel()
                => new CalendarPermissionDeniedViewModel(NavigationService, PermissionsChecker, RxActionFactory);
        }

        public sealed class TheConstructor : CalendarPermissionDeniedViewModelTest
        {
            [Theory, LogIfTooSlow]
            [ConstructorData]
            public void ThrowsIfAnyOfTheArgumentsIsNull(
                bool useNavigationService,
                bool usePermissionsChecker,
                bool useRxActionFactory)
            {
                Action tryingToConstructWithEmptyParameters =
                    () => new CalendarPermissionDeniedViewModel(
                        useNavigationService ? NavigationService : null,
                        usePermissionsChecker ? PermissionsChecker : null,
                        useRxActionFactory ? RxActionFactory : null
                    );

                tryingToConstructWithEmptyParameters.Should().Throw<ArgumentNullException>();
            }
        }

        public sealed class TheEnableAccessAction : CalendarPermissionDeniedViewModelTest
        {
            [Fact]
            public void OpensAppSettings()
            {
                ViewModel.EnableAccess.Execute();

                View.Received().OpenAppSettings();
            }
        }

        public sealed class TheViewAppearedMethod : CalendarPermissionDeniedViewModelTest
        {
            [Fact]
            public async Task ClosesWhenPermissionWasGranted()
            {
                PermissionsChecker.CalendarPermissionGranted.Returns(Observable.Return(true));
                ViewModel.ViewAppeared();

                TestScheduler.Start();

                await View.Received().Close();
            }

            [Fact]
            public async Task DoesNothingWhenPermissionWasNotGranted()
            {
                PermissionsChecker.CalendarPermissionGranted.Returns(Observable.Return(false));
                ViewModel.ViewAppeared();

                TestScheduler.Start();

                await View.DidNotReceive().Close();
            }
        }
    }
}