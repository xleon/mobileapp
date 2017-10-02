using System;
using System.Collections.Generic;

namespace Toggl.Foundation.DTOs
{
    public struct EditTimeEntryDto
    {
        public long Id { get; set; }

        public string Description { get; set; }

        public DateTimeOffset StartTime { get; set; }

        public DateTimeOffset? StopTime { get; set; }

        public long? ProjectId { get; set; }

        public bool Billable { get; set; }

        public IEnumerable<long> TagIds { get; set; }
    }
}
