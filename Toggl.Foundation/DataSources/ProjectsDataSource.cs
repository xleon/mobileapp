using System;
using System.Collections.Generic;
using Toggl.Multivac;
using Toggl.Multivac.Models;
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

        public IObservable<IEnumerable<IProject>> GetAll()
            => repository.GetAll();

        public IObservable<IProject> GetById(int id)
            => repository.GetById(id);
    }
}
