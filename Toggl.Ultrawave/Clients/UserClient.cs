using System.Collections.Generic;
using System.Text;
using Toggl.Multivac;
using Toggl.Ultrawave.Network;

namespace Toggl.Ultrawave.Clients
{
    internal sealed class UserClient : BaseClient, IUserClient
    {
        private readonly UserEndpoints endPoints;

        public UserClient(UserEndpoints endPoints, IApiClient apiClient, IJsonSerializer serializer)
            : base(apiClient, serializer)
        {
            Ensure.ArgumentIsNotNull(endPoints, nameof(endPoints));

            this.endPoints = endPoints;
        }

        public ICall<User> Get(string username, string password)
        {
            var header = GetAuthHeader(username, password);
            var call = CreateCall<User>(endPoints.Get, header);
            return call;
        }
   }
}
