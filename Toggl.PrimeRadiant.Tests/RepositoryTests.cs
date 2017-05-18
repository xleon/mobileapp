﻿using System;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Toggl.PrimeRadiant.Exceptions;
using Toggl.Multivac.Extensions;
using Xunit;

namespace Toggl.PrimeRadiant.Tests
{
    public abstract class RepositoryTests<TTestModel> : BaseStorageTests<TTestModel>
        where TTestModel : class, ITestModel, IDatabaseSyncable, new()
    {
        protected sealed override IObservable<TTestModel> Create(TTestModel testModel)
            => Repository.Create(testModel);

        protected sealed override IObservable<TTestModel> Update(TTestModel testModel)
            => Repository.Update(testModel);

        protected sealed override IObservable<Unit> Delete(TTestModel testModel)
            => Repository.Delete(testModel);

        protected abstract IRepository<TTestModel> Repository { get; }

        [Fact]
        public void TheGetByIdMethodThrowsIfThereIsNoEntityWithThatIdInTheRepository()
        {
            Func<Task> callingGetByIdInAnEmptyRepository = 
                async () => await Repository.GetById(-1);

            callingGetByIdInAnEmptyRepository
                .ShouldThrow<EntityNotFoundException>();
        }

        [Fact]
        public async Task TheGetByIdMethodAlwaysReturnsASingleElement()
        {
            var testEntity = new TTestModel();
            await Repository.Create(testEntity);

            var element = await Repository.GetById(testEntity.Id).SingleAsync();
            element.Should().Be(testEntity);
        }

        [Fact]
        public async Task TheGetAllMethodReturnsAnEmptyListIfThereIsNothingOnTheRepository()
        {
            var entities = await Repository.GetAll(_ => true);
            entities.Count().Should().Be(0);
        }

        [Fact]
        public async Task TheGetAllMethodReturnsAllItemsThatMatchTheQuery()
        {
            const int numberOfItems = 5;

            await Enumerable.Range(0, numberOfItems)
                            .Select(GetModelWith)
                            .Select(async model => await Repository.Create(model))
                            .Apply(Task.WhenAll);
            
            var entities = await Repository.GetAll(_ => true);
            entities.Count().Should().Be(numberOfItems);
        }
    }
}
