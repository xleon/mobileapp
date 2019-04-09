﻿using System;
 using Toggl.Shared.Models;

namespace Toggl.PrimeRadiant.Models
{
    public interface IDatabaseSinceParameter : IIdentifiable
    {
        DateTimeOffset? Since { get; }
    }
}
