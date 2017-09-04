using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using FluentAssertions;
using NSubstitute;
using Toggl.Foundation.Models;
using Toggl.Foundation.Sync;
using Toggl.Foundation.Sync.States;
using Toggl.Multivac.Models;
using Toggl.PrimeRadiant;
using Toggl.PrimeRadiant.Models;
using Xunit;

namespace Toggl.Foundation.Tests.Sync.States
{
    public abstract class PersistStateTests
    {
        private readonly ITheStartMethodHelper testHelper;

        protected PersistStateTests(ITheStartMethodHelper helper)
        {
            testHelper = helper;
        }

        [Fact]
        public void EmitsTransitionToPersistFinished()
            => testHelper.EmitsTransitionToPersistFinished();

        [Fact]
        public void ThrowsIfFetchObservablePublishesTwice()
            => testHelper.ThrowsIfFetchObservablePublishesTwice();

        [Fact]
        public void TriggersBatchUpdate()
            => testHelper.TriggersBatchUpdate();

        [Fact]
        public void DoesNotUpdateSinceParametersWhenNothingIsFetched()
            => testHelper.DoesNotUpdateSinceParametersWhenNothingIsFetched();

        [Fact]
        public void UpdatesSinceParametersOfTheFetchedEntity()
            => testHelper.UpdatesSinceParametersOfTheFetchedEntity();

        [Fact]
        public void SelectsTheLatestAtValue()
            => testHelper.SelectsTheLatestAtValue();

        [Fact]
        public void PassesTheNewSinceParametersThroughTheTransition()
            => testHelper.PassesTheNewSinceParametersThroughTheTransition();

        [Fact]
        public void SinceDatesAreNotUpdatedWhenBatchUpdateThrows()
            => testHelper.SinceDatesAreNotUpdatedWhenBatchUpdateThrows();

        [Fact]
        public void ThrowsWhenBatchUpdateThrows()
            => testHelper.ThrowsWhenBatchUpdateThrows();

        public interface ITheStartMethodHelper
        {
            void EmitsTransitionToPersistFinished();

            void ThrowsIfFetchObservablePublishesTwice();

            void TriggersBatchUpdate();

            void ThrowsWhenBatchUpdateThrows();

            void DoesNotUpdateSinceParametersWhenNothingIsFetched();

            void UpdatesSinceParametersOfTheFetchedEntity();

            void SelectsTheLatestAtValue();

            void SinceDatesAreNotUpdatedWhenBatchUpdateThrows();

            void PassesTheNewSinceParametersThroughTheTransition();
        }

        internal abstract class TheStartMethod<TState, TInterface, TDatabaseInterface> : ITheStartMethodHelper
            where TDatabaseInterface : TInterface
            where TState : BasePersistState<TInterface, TDatabaseInterface>
        {
            private readonly ITogglDatabase database;
            private readonly TState state;

            protected TheStartMethod()
            {
                database = Substitute.For<ITogglDatabase>();
                state = CreateState(database);
            }

            public void EmitsTransitionToPersistFinished()
            {
                var observables = CreateObservables();

                var transition = state.Start(observables).SingleAsync().Wait();

                transition.Result.Should().Be(state.FinishedPersisting);
            }

            public void ThrowsIfFetchObservablePublishesTwice()
            {
                var fetchObservables = CreateObservablesWhichFetchesTwice();

                Action fetchTwice = () => state.Start(fetchObservables).Wait();

                fetchTwice.ShouldThrow<InvalidOperationException>();
            }

            public void TriggersBatchUpdate()
            {
                var at = new DateTimeOffset(2017, 09, 01, 12, 34, 56, TimeSpan.Zero);
                var entities = CreateComplexListWhereTheLastUpdateEntityIsDeleted(at);
                var observables = CreateObservables(null, entities);

                state.Start(observables).SingleAsync().Wait();

                AssertBatchUpdateWasCalled(database, entities);
            }

            public void ThrowsWhenBatchUpdateThrows()
            {
                var observables = CreateObservables();
                SetupDatabaseBatchUpdateToThrow(database, () => new TestException());

                Action startingState = () => state.Start(observables).SingleAsync().Wait();

                startingState.ShouldThrow<TestException>();
            }

            public void DoesNotUpdateSinceParametersWhenNothingIsFetched()
            {
                var at = new DateTimeOffset(2017, 09, 01, 12, 34, 56, TimeSpan.Zero);
                var oldSinceParameters = new SinceParameters(
                    null,
                    workspaces: at,
                    clients: at.AddDays(1),
                    projects: at.AddDays(2),
                    tags: at.AddDays(3),
                    tasks: at.AddDays(4),
                    timeEntries: at.AddDays(5)
                );
                var observables = CreateObservables(oldSinceParameters);

                state.Start(observables).SingleAsync().Wait();

                database.SinceParameters.Received().Set(Arg.Is<ISinceParameters>(
                    newSinceParameters =>
                        newSinceParameters.Workspaces == oldSinceParameters.Workspaces
                        && newSinceParameters.Projects == oldSinceParameters.Projects
                        && newSinceParameters.Clients == oldSinceParameters.Clients
                        && newSinceParameters.Tags == oldSinceParameters.Tags
                        && newSinceParameters.Tasks == oldSinceParameters.Tasks));
            }

            public void UpdatesSinceParametersOfTheFetchedEntity()
            {
                var oldAt = new DateTimeOffset(2017, 09, 01, 12, 34, 56, TimeSpan.Zero);
                var newAt = new DateTimeOffset(2017, 10, 01, 12, 34, 56, TimeSpan.Zero);
                var oldSinceParameters = new SinceParameters(null, oldAt);
                var entities = CreateListWithOneItem(newAt);
                var observables = CreateObservables(oldSinceParameters, entities);
                SetupDatabaseBatchUpdateMocksToReturnDatabaseEntitiesAndFilterOutDeletedEntities(database, entities);

                state.Start(observables).SingleAsync().Wait();

                database.SinceParameters.Received().Set(Arg.Is<ISinceParameters>(
                    newSinceParameters => OtherSinceDatesDidntChange(oldSinceParameters, newSinceParameters, newAt)));
            }

            public void SelectsTheLatestAtValue()
            {
                var at = new DateTimeOffset(2017, 09, 01, 12, 34, 56, TimeSpan.Zero);
                var oldSinceParameters = new SinceParameters(null, at.AddMonths(-1));
                var entities = CreateComplexListWhereTheLastUpdateEntityIsDeleted(at);
                var observables = CreateObservables(oldSinceParameters, entities);
                SetupDatabaseBatchUpdateMocksToReturnDatabaseEntitiesAndFilterOutDeletedEntities(database, entities);

                state.Start(observables).SingleAsync().Wait();

                database.SinceParameters.Received().Set(Arg.Is<ISinceParameters>(
                    (newSinceParameters) => OtherSinceDatesDidntChange(oldSinceParameters, newSinceParameters, at)));
            }

            public void SinceDatesAreNotUpdatedWhenBatchUpdateThrows()
            {
                var at = new DateTimeOffset(2017, 09, 01, 12, 34, 56, TimeSpan.Zero);
                var oldSinceParameters = new SinceParameters(null, at);
                var entities = CreateComplexListWhereTheLastUpdateEntityIsDeleted(at);
                var observables = CreateObservables(oldSinceParameters, entities);
                SetupDatabaseBatchUpdateToThrow(database, () => new TestException());

                try
                {
                    state.Start(observables).SingleAsync().Wait();
                }
                catch (TestException) { }

                database.SinceParameters.DidNotReceiveWithAnyArgs().Set(null);
            }

            public void PassesTheNewSinceParametersThroughTheTransition()
            {
                var at = new DateTimeOffset(2017, 09, 01, 12, 34, 56, TimeSpan.Zero);
                var oldSinceParameters = new SinceParameters(null, at);
                var entities = CreateComplexListWhereTheLastUpdateEntityIsDeleted(at);
                var observables = CreateObservables(oldSinceParameters, entities);
                SetupDatabaseBatchUpdateMocksToReturnDatabaseEntitiesAndFilterOutDeletedEntities(database, entities);

                var transition = (Transition<FetchObservables>)state.Start(observables).SingleAsync().Wait();

                transition.Parameter.SinceParameters.Should()
                    .Match((ISinceParameters newSinceParameters) => OtherSinceDatesDidntChange(oldSinceParameters, newSinceParameters, at));
            }

            protected FetchObservables CreateFetchObservables(
                FetchObservables old, ISinceParameters sinceParameters,
                IObservable<List<IWorkspace>> workspaces = null, IObservable<List<IClient>> clients = null,
                IObservable<List<IProject>> projects = null, IObservable<List<ITimeEntry>> timeEntries = null)
            => new FetchObservables(
                old?.SinceParameters ?? sinceParameters,
                old?.Workspaces ?? workspaces,
                old?.Clients ?? clients,
                old?.Projects ?? projects,
                old?.TimeEntries ?? timeEntries);

            protected abstract TState CreateState(ITogglDatabase database);

            protected abstract List<TInterface> CreateEmptyList();

            protected abstract List<TInterface> CreateListWithOneItem(DateTimeOffset? at = null);

            protected abstract List<TInterface> CreateComplexListWhereTheLastUpdateEntityIsDeleted(DateTimeOffset? at);

            protected abstract FetchObservables CreateObservablesWhichFetchesTwice();

            protected abstract bool OtherSinceDatesDidntChange(ISinceParameters old, ISinceParameters next, DateTimeOffset at);

            protected abstract FetchObservables CreateObservables(ISinceParameters since = null, List<TInterface> entities = null);

            protected abstract void SetupDatabaseBatchUpdateMocksToReturnDatabaseEntitiesAndFilterOutDeletedEntities(ITogglDatabase database, List<TInterface> entities = null);

            protected abstract void SetupDatabaseBatchUpdateToThrow(ITogglDatabase database, Func<Exception> exceptionFactory);

            protected abstract void AssertBatchUpdateWasCalled(ITogglDatabase database, List<TInterface> entities = null);
        }
    }
}
