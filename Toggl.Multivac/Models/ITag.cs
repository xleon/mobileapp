using System;

namespace Toggl.Multivac.Models
{
    public interface ITag : IBaseModel
    {
        int WorkspaceId { get; }

        string Name { get; }

        DateTimeOffset At { get; }
    }
}
