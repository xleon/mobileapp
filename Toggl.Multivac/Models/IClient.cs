namespace Toggl.Multivac.Models
{
    public interface IClient : IBaseModel
    {
        int WorkspaceId { get; }

        string Name { get; }

        string At { get; }
    }
}
