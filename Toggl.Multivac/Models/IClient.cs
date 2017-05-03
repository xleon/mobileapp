using System;

namespace Toggl.Multivac.Models
{
    public interface IClient : IBaseModel
    {
        int WorkspaceId { get; }

        string Name { get; }

        DateTimeOffset At { get; }
    }
}
