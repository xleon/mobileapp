using System;
using System.Collections.Generic;
using Toggl.Ultrawave.Models;

namespace Toggl.Ultrawave.ApiClients
{
    public interface ITagsApi
    {
        IObservable<List<Tag>> GetAll();
    }
}