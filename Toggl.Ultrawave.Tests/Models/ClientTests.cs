using System;
using Toggl.Ultrawave.Models;

namespace Toggl.Ultrawave.Tests.Models
{
    public class ClientTests
    {
        public class TheClientModel : BaseModelTests<Client>
        {
            protected override string ValidJson
                => "{\"id\":23741667,\"wid\":1427273,\"name\":\"Test\",\"at\":\"2014-04-25T10:10:13+00:00\",\"server_deleted_at\":null}";

            protected override Client ValidObject => new Client
            {
                Id = 23741667,
                WorkspaceId = 1427273,
                Name = "Test",
                At = new DateTimeOffset(2014, 04, 25, 10, 10, 13, TimeSpan.Zero),
                ServerDeletedAt = null
            };
        }
    }
}
