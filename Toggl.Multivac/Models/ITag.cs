using System;

namespace Toggl.Multivac.Models
{
    public interface ITag : IBaseModel
    {
        long WorkspaceId { get; }

        string Name { get; }

        DateTimeOffset At { get; }
    }
}
