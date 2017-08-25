using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Toggl.PrimeRadiant.Realm;

namespace Toggl.PrimeRadiant.Tests.Realm
{
    public sealed class TestAdapter : IRealmAdapter<TestModel>
    {
        private readonly List<TestModel> list = new List<TestModel>();

        public TestModel Create(TestModel entity)
        {
            if (list.Find(e => e.Id == entity.Id) != null)
                throw new InvalidOperationException();

            list.Add(entity);

            return entity;
        }

        public TestModel Update(TestModel entity)
        {
            var index = list.FindIndex(e => e.Id == entity.Id);

            if (index == -1)
                throw new InvalidOperationException();

            list[index] = entity;

            return entity;
        }

        public IQueryable<TestModel> GetAll()
            => list.AsQueryable();

        public void Delete(TestModel entity)
        {
            var worked = list.Remove(entity);
            if (worked) return;

            throw new InvalidOperationException();
        }

        public IEnumerable<TestModel> BatchUpdate(IEnumerable<TestModel> entities, Func<TestModel, Expression<Func<TestModel, bool>>> matchEntity, Func<TestModel, TestModel, ConflictResolutionMode> conflictResolution)
        {
            throw new NotImplementedException();
        }
    }
}
