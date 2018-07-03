using System;
using System.Reactive.Linq;
using FluentAssertions;
using NSubstitute;
using Toggl.Foundation.DataSources.Interfaces;
using Toggl.Foundation.Sync;
using Toggl.Foundation.Sync.States.Push;
using Toggl.Foundation.Tests.Sync.States.Push.BaseStates;
using Toggl.PrimeRadiant;
using Toggl.Ultrawave.ApiClients;
using Xunit;

namespace Toggl.Foundation.Tests.Sync.States.Push
{
    public sealed class CreateEntityStateTests : BasePushEntityStateTests
    {
        private readonly ICreatingApiClient<ITestModel> api;

        private readonly IDataSource<IThreadSafeTestModel, IDatabaseTestModel> dataSource;

        public CreateEntityStateTests()
            : this(Substitute.For<IDataSource<IThreadSafeTestModel, IDatabaseTestModel>>())
        {
        }

        private CreateEntityStateTests(IDataSource<IThreadSafeTestModel, IDatabaseTestModel> dataSource)
        {
            api = Substitute.For<ICreatingApiClient<ITestModel>>();
            this.dataSource = dataSource;
        }

        [Fact, LogIfTooSlow]
        public void ReturnsSuccessfulTransitionWhenEverythingWorks()
        {
            var state = (CreateEntityState<ITestModel, IDatabaseTestModel, IThreadSafeTestModel>)CreateState();
            var entity = new TestModel(-1, SyncStatus.SyncFailed);
            var withPositiveId = new TestModel(Math.Abs(entity.Id), SyncStatus.InSync);
            api.Create(Arg.Any<ITestModel>())
                .Returns(Observable.Return(withPositiveId));
            dataSource.OverwriteIfOriginalDidNotChange(Arg.Any<IThreadSafeTestModel>(), Arg.Any<IThreadSafeTestModel>())
                .Returns(x => Observable.Return(
                    new UpdateResult<IThreadSafeTestModel>(((IThreadSafeTestModel)x[1]).Id, (IThreadSafeTestModel)x[1])));

            var transition = state.Start(entity).SingleAsync().Wait();
            var persistedEntity = ((Transition<IThreadSafeTestModel>)transition).Parameter;

            transition.Result.Should().Be(state.Finished);
            persistedEntity.Id.Should().NotBe(entity.Id);
            persistedEntity.Id.Should().BeGreaterThan(0);
            persistedEntity.SyncStatus.Should().Be(SyncStatus.InSync);
        }

        [Fact, LogIfTooSlow]
        public void TriesToSafelyOverwriteLocalDataWithDataFromTheServer()
        {
            var state = (CreateEntityState<ITestModel, IDatabaseTestModel, IThreadSafeTestModel>)CreateState();
            var entity = new TestModel(-1, SyncStatus.SyncFailed);
            var withPositiveId = new TestModel(Math.Abs(entity.Id), SyncStatus.InSync);
            api.Create(entity)
                .Returns(Observable.Return(withPositiveId));

            state.Start(entity).SingleAsync().Wait();

            dataSource
                .Received()
                .OverwriteIfOriginalDidNotChange(
                    Arg.Is<IThreadSafeTestModel>(original => original.Id == entity.Id),
                    Arg.Is<IThreadSafeTestModel>(fromServer => fromServer.Id == withPositiveId.Id));
        }

        [Fact, LogIfTooSlow]
        public void UpdateIsCalledWithCorrectParameters()
        {
            var state = (CreateEntityState<ITestModel, IDatabaseTestModel, IThreadSafeTestModel>)CreateState();
            var entity = new TestModel(-1, SyncStatus.SyncFailed);
            var withPositiveId = new TestModel(Math.Abs(entity.Id), SyncStatus.InSync);
            api.Create(entity)
                .Returns(Observable.Return(withPositiveId));

            state.Start(entity).SingleAsync().Wait();

            dataSource
                .Received()
                .OverwriteIfOriginalDidNotChange(Arg.Is<IThreadSafeTestModel>(model => model.Id == entity.Id), Arg.Is<IThreadSafeTestModel>(model => model.Id == withPositiveId.Id));
        }

        [Fact, LogIfTooSlow]
        public void ReturnsTheEntityChangedTransitionWhenEntityChangesLocally()
        {
            var state = (CreateEntityState<ITestModel, IDatabaseTestModel, IThreadSafeTestModel>)CreateState();
            var at = new DateTimeOffset(2017, 9, 1, 12, 34, 56, TimeSpan.Zero);
            var entity = new TestModel { Id = 1, At = at, SyncStatus = SyncStatus.SyncNeeded };
            api.Create(Arg.Any<ITestModel>())
                .Returns(Observable.Return(entity));
            dataSource
                .OverwriteIfOriginalDidNotChange(Arg.Any<IThreadSafeTestModel>(), Arg.Any<IThreadSafeTestModel>())
                .Returns(Observable.Return(new IgnoreResult<IThreadSafeTestModel>(entity.Id)));
            dataSource.ChangeId(Arg.Any<long>(), entity.Id).Returns(Observable.Return(entity));

            var transition = state.Start(entity).SingleAsync().Wait();
            var parameter = ((Transition<IThreadSafeTestModel>)transition).Parameter;

            transition.Result.Should().Be(state.EntityChanged);
            parameter.Id.Should().Be(entity.Id);
        }

        protected override BasePushEntityState<IThreadSafeTestModel> CreateState()
            => new CreateEntityState<ITestModel, IDatabaseTestModel, IThreadSafeTestModel>(api, dataSource, TestModel.From);

        protected override void PrepareApiCallFunctionToThrow(Exception e)
        {
            api.Create(Arg.Any<ITestModel>())
                .Returns(_ => Observable.Throw<ITestModel>(e));
        }

        protected override void PrepareDatabaseOperationToThrow(Exception e)
        {
            dataSource.OverwriteIfOriginalDidNotChange(Arg.Any<IThreadSafeTestModel>(), Arg.Any<IThreadSafeTestModel>())
                .Returns(_ => Observable.Throw<IConflictResolutionResult<IThreadSafeTestModel>>(e));
        }
    }
}
