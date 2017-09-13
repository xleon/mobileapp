using System;
using System.Reactive.Linq;
using Toggl.Foundation.Models;
using Toggl.PrimeRadiant;
using Toggl.PrimeRadiant.Models;
using Toggl.Ultrawave;

namespace Toggl.Foundation.Sync.States
{
    internal sealed class UpdateTimeEntryState : BaseUpdateEntityState<IDatabaseTimeEntry>
    {
        public UpdateTimeEntryState(ITogglApi api, ITogglDatabase database) : base(api, database)
        {
        }

        protected override IRepository<IDatabaseTimeEntry> GetRepository(ITogglDatabase database)
            => database.TimeEntries;

        protected override bool HasChanged(IDatabaseTimeEntry original, IDatabaseTimeEntry current)
            => original.At < current.At;

        protected override IObservable<IDatabaseTimeEntry> Update(ITogglApi api, IDatabaseTimeEntry entity)
            => api.TimeEntries.Update(entity).Select(TimeEntry.Clean);
    }
}
