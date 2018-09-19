using System;
using Toggl.Foundation.DTOs;
using Toggl.Foundation.Models.Interfaces;

namespace Toggl.Foundation.Interactors
{
    public sealed partial class InteractorFactory : IInteractorFactory
    {
        public IInteractor<IObservable<bool>> IsBillableAvailableForProject(long projectId)
            => new IsBillableAvailableForProjectInteractor(dataSource, projectId);

        public IInteractor<IObservable<bool>> ProjectDefaultsToBillable(long projectId)
            => new ProjectDefaultsToBillableInteractor(dataSource, projectId);

        public IInteractor<IObservable<IThreadSafeProject>> CreateProject(CreateProjectDTO dto)
            => new CreateProjectInteractor(idProvider, timeService, dataSource.Projects, dto);
    }
}
