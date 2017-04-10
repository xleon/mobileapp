using System.Net.Http;
using System.Threading.Tasks;
using Toggl.Multivac;
using Toggl.Ultrawave.Extensions;

namespace Toggl.Ultrawave.Network
{
    internal sealed class ApiClient : IApiClient
    {
        private const string DefaultContentType = "text/plain";

        private readonly HttpClient httpClient;
        
        public ApiClient(HttpClient httpClient)
        {
            Ensure.ArgumentIsNotNull(httpClient, nameof(httpClient));

            this.httpClient = httpClient;
        }

        public async Task<IResponse> Send(IRequest request)
        {
            Ensure.ArgumentIsNotNull(request, nameof(request));

            var requestMessage = createRequestMessage(request);
            var responseMessage = await httpClient.SendAsync(requestMessage).ConfigureAwait(false);

            var response = await createResponse(responseMessage).ConfigureAwait(false);
            return response;
        }

        public void Dispose()
        {
            httpClient.Dispose();
        }

        private HttpRequestMessage createRequestMessage(IRequest request)
        {
            Ensure.ArgumentIsNotNull(request, nameof(request));

            var requestMessage = new HttpRequestMessage(request.HttpMethod, request.Endpoint);
            requestMessage.Headers.AddRange(request.Headers);

            switch (request.Body)
            {
                case string json:
                    requestMessage.Content = new JsonContent(json);
                    break;
                case byte[] bytes:
                    requestMessage.Content = new ByteArrayContent(bytes);
                    break;
            }

            return requestMessage;
        }

        private async Task<IResponse> createResponse(HttpResponseMessage responseMessage)
        {
            Ensure.ArgumentIsNotNull(responseMessage, nameof(responseMessage));

            var rawResponseString = "";
            var isSuccess = responseMessage.IsSuccessStatusCode;
            var contentType = responseMessage.Content?.Headers?.ContentType?.MediaType ?? DefaultContentType;

            using (var content = responseMessage.Content)
            {
                if (content != null)
                {
                    rawResponseString = await content.ReadAsStringAsync().ConfigureAwait(false);
                }
            }

            var response = new Response(rawResponseString, isSuccess, contentType, responseMessage.StatusCode);
            return response;
        }
    }
}
