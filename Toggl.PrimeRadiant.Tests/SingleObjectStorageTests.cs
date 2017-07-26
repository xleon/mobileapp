using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Toggl.PrimeRadiant.Exceptions;
using Xunit;

namespace Toggl.PrimeRadiant.Tests
{
    public abstract class SingleObjectStorageTests<TTestModel> : BaseStorageTests<TTestModel>
        where TTestModel : class, ITestModel, IDatabaseSyncable, new()
    {
        protected sealed override IObservable<TTestModel> Create(TTestModel testModel)
            => Storage.Create(testModel);

        protected sealed override IObservable<TTestModel> Update(TTestModel testModel)
            => Storage.Update(testModel);

        protected sealed override IObservable<Unit> Delete(TTestModel testModel)
            => Storage.Delete();

        protected abstract ISingleObjectStorage<TTestModel> Storage { get; }

        [Fact]
        public void TheSingleMethodThrowsIfThereIsNoDataInTheRepository()
        {
            Func<Task> callingGetLastInAnEmptyRepository =
                async () => await Storage.Single();

            callingGetLastInAnEmptyRepository
                .ShouldThrow<EntityNotFoundException>();
        }

        [Fact]
        public async Task TheSingleMethodAlwaysReturnsASingleElement()
        {
            var testEntity = new TTestModel();
            await Storage.Create(testEntity);

            var element = await Storage.Single();
            element.Should().Be(testEntity);
        }

        [Fact]
        public async Task TheCreateModelThrowsIfAnItemAlreadyExistsRegardlessOfId()
        {
            var testEntity = new TTestModel();
            await Storage.Create(GetModelWith(1));

            Func<Task> callingCreateASecondTime =
                async () => await Storage.Create(GetModelWith(2));

            callingCreateASecondTime
                .ShouldThrow<EntityAlreadyExistsException>();
        }
    }
}
