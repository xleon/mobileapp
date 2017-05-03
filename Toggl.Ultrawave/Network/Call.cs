using System.Threading.Tasks;
using Toggl.Ultrawave.Serialization;
using Toggl.Multivac;

namespace Toggl.Ultrawave.Network
{
    internal sealed class Call<T> : BaseCall<T>
    {
        readonly IRequest request;
        readonly IApiClient apiClient;
        readonly IJsonSerializer serializer;

        public Call(IRequest request, IApiClient apiClient, IJsonSerializer serializer)
        {
            Ensure.ArgumentIsNotNull(request, nameof(request));
            Ensure.ArgumentIsNotNull(apiClient, nameof(apiClient));
            Ensure.ArgumentIsNotNull(serializer, nameof(serializer));

            this.request = request;
            this.apiClient = apiClient;
            this.serializer = serializer;
        }

        protected override async Task<IApiResponse<T>> SafeExecute()
        {
            var response = await apiClient.Send(request).ConfigureAwait(false);
            if (response.IsSuccess)
            {
                var data = await serializer.Deserialize<T>(response.RawData).ConfigureAwait(false);
                var apiResponse = ApiResponse.FromData(data);
                return apiResponse;
            }
            else
            {
                //TODO: Treat different error responses here. We need to check those as we create our clients.
                var apiResponse = ApiResponse<T>.FromErrorMessage(response.RawData);
                return apiResponse;
            }
        }
    }
}
