using System;
using Toggl.Shared;

namespace Toggl.Core.Services
{
    public interface IRemoteConfigService
    {
        IObservable<RatingViewConfiguration> RatingViewConfiguration { get; }
    }
}
