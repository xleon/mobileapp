using System;
using System.Collections.Generic;
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
            => repository.GetAll();
    }
}
