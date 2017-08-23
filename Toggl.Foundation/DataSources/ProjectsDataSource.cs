using System;
using System.Collections.Generic;
using Toggl.Multivac;
using Toggl.PrimeRadiant;
using Toggl.PrimeRadiant.Models;

namespace Toggl.Foundation.DataSources
{
    public class ProjectsDataSource : IProjectsSource
    {
        private readonly IRepository<IDatabaseProject> repository;

        public ProjectsDataSource(IRepository<IDatabaseProject> repository)
        {
            Ensure.Argument.IsNotNull(repository, nameof(repository));

            this.repository = repository;
        }

        public IObservable<IEnumerable<IDatabaseProject>> GetAll()
            => repository.GetAll();

        public IObservable<IEnumerable<IDatabaseProject>> GetAll(Func<IDatabaseProject, bool> predicate)
            => repository.GetAll(predicate);

        public IObservable<IDatabaseProject> GetById(long id)
            => repository.GetById(id);
    }
}
