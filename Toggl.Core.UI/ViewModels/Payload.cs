using System;
using System.Linq;
using Toggl.Core.Models.Interfaces;
using Toggl.Shared;

namespace Toggl.Core.UI.ViewModels
{
    [Preserve(AllMembers = true)]
    public class SharePayload
    {
        public long WorkspaceId { get; set; }
        public string WorkspaceName { get; set; }
        public long? ProjectId { get; set; }
        public string ProjectName { get; set; }
        public long? ClientId { get; set; }
        public string ClientName { get; set; }
        public long? TaskId { get; set; }
        public string TaskName { get; set; }
        public ShareTag[] Tags { get; set; }
        public string Description { get; set; }
        public bool IsBillable { get; set; }
        public DateTimeOffset Start { get; set; }
        public DateTimeOffset? Stop { get; set; }
    }

    [Preserve(AllMembers = true)]
    public class ShareTag
    {
        public ShareTag()
        {
        }
        
        public long Id { get; set; }
        public string Name { get; set; }
    }
}
