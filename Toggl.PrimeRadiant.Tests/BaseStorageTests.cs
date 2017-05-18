using System;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Toggl.PrimeRadiant.Exceptions;
using Xunit;

namespace Toggl.PrimeRadiant.Tests
{
    public abstract class BaseStorageTests<TTestModel>
        where TTestModel : class, ITestModel, IDatabaseSyncable, new()
    {
        protected abstract IObservable<TTestModel> Create(TTestModel testModel);
        protected abstract IObservable<TTestModel> Update(TTestModel testModel);
        protected abstract IObservable<Unit> Delete(TTestModel testModel);

        protected abstract TTestModel GetModelWith(int id);

        [Fact]
        public void TheUpdateMethodThrowsIfThereIsNoEntityWithThatIdInTheRepository()
        {
            Func<Task> callingUpdateInAnEmptyRepository =
                async () => await Update(new TTestModel());

            callingUpdateInAnEmptyRepository
                .ShouldThrow<EntityNotFoundException>();
        }

        [Fact]
        public async Task TheUpdateMethodAlwaysReturnsASingleElement()
        {
            var testEntity = new TTestModel();
            await Create(testEntity);

            var element = await Update(testEntity).SingleAsync();
            element.Should().Be(testEntity);
        }

        [Fact]
        public void TheDeleteMethodThrowsIfThereIsNoEntityWithThatIdInTheRepository()
        {
            Func<Task> callingDeleteInAnEmptyRepository =
                async () => await Delete(new TTestModel());

            callingDeleteInAnEmptyRepository
                .ShouldThrow<EntityNotFoundException>();
        }
    }
}
