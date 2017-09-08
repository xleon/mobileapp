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

        public TestModel Update(long id, TestModel entity)
        {
            var index = list.FindIndex(e => e.Id == id);

            if (index == -1)
                throw new InvalidOperationException();

            list[index] = entity;

            return entity;
        }

        public IQueryable<TestModel> GetAll()
            => list.AsQueryable();

        public void Delete(long id)
        {
            var entity = list.Find(e => e.Id == id);
            var worked = list.Remove(entity);
            if (worked) return;

            throw new InvalidOperationException();
        }

        public IEnumerable<TestModel> BatchUpdate(IEnumerable<(long Id, TestModel Entity)> entities, Func<(long Id, TestModel Entity), Expression<Func<TestModel, bool>>> matchEntity, Func<TestModel, TestModel, ConflictResolutionMode> conflictResolution)
        {
            throw new NotImplementedException();
        }
    }
}
