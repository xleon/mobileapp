using System;
using System.Collections.Generic;
using Toggl.Multivac.Models;

namespace Toggl.Ultrawave.ApiClients
{
    public interface ITagsApi
    {
        IObservable<List<ITag>> GetAll();
    }
}