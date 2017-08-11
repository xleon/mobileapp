﻿using System;

namespace Toggl.PrimeRadiant.Models
{
    public interface ISinceParameters
    {
        DateTimeOffset? Workspaces { get; }
        DateTimeOffset? Tags { get; }
        DateTimeOffset? Clients { get; }
        DateTimeOffset? Projects { get; }
        DateTimeOffset? Tasks { get; }
        DateTimeOffset? TimeEntries { get; }
    }
}
