using System;
using Toggl.Shared;

namespace Toggl.Foundation.Services
{
    public interface IRemoteConfigService
    {
        IObservable<RatingViewConfiguration> RatingViewConfiguration { get; }
    }
}
