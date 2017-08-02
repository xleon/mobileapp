using System;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;
using Toggl.Foundation.Models;
using Toggl.Multivac;
using Toggl.Multivac.Models;
using Toggl.PrimeRadiant;
using Toggl.PrimeRadiant.Models;

namespace Toggl.Foundation.DataSources
{
    internal sealed class TimeEntriesDataSource : ITimeEntriesSource
    {
        private readonly IRepository<IDatabaseTimeEntry> repository;

        public TimeEntriesDataSource(IRepository<IDatabaseTimeEntry> repository)
        {
            Ensure.Argument.IsNotNull(repository, nameof(repository));

            this.repository = repository;
        }

        public IObservable<IEnumerable<ITimeEntry>> GetAll()
            => repository.GetAll(te => !te.IsDeleted);

        public IObservable<Unit> Delete(int id)
            => repository.GetById(id)
                         .Select(TimeEntry.DirtyDeleted)
                         .SelectMany(repository.Update)
                         .IgnoreElements()
                         .Cast<Unit>();
    }
}
