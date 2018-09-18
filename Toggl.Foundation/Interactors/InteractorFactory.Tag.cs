using System;
using Toggl.Foundation.Models.Interfaces;

namespace Toggl.Foundation.Interactors
{
    public sealed partial class InteractorFactory
    {
        public IInteractor<IObservable<IThreadSafeTag>> CreateTag(string tagName, long workspaceId)
            => new CreateTagInteractor(idProvider, timeService, dataSource.Tags, tagName, workspaceId);
    }
}
