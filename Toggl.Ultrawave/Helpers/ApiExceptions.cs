using Toggl.Ultrawave.Exceptions;
using Toggl.Ultrawave.Network;

namespace Toggl.Ultrawave.Helpers
{
    internal static class ApiExceptions
    {
        public static ApiException ForResponse(IResponse response)
        {
            var rawData = response.RawData;
            switch (response.StatusCode)
            {
                // Client errors
                case ApiDeprecatedException.CorrespondingHttpCode:
                    return new ApiDeprecatedException(rawData);
                case BadRequestException.CorrespondingHttpCode:
                    return new BadRequestException(rawData);
                case ClientDeprecatedException.CorrespondingHttpCode:
                    return new ClientDeprecatedException(rawData);
                case ForbiddenException.CorrespondingHttpCode:
                    return new ForbiddenException(rawData);
                case NotFoundException.CorrespondingHttpCode:
                    return new NotFoundException(rawData);
                case PaymentRequiredException.CorrespondingHttpCode:
                    return new PaymentRequiredException(rawData);
                case RequestEntityTooLargeException.CorrespondingHttpCode:
                    return new RequestEntityTooLargeException(rawData);
                case TooManyRequestsException.CorrespondingHttpCode:
                    return new TooManyRequestsException(rawData);
                case UnauthorizedException.CorrespondingHttpCode:
                    return new UnauthorizedException(rawData);

                // Server errors
                case InternalServerErrorException.CorrespondingHttpCode:
                    return new InternalServerErrorException(rawData);
                case NotImplementedException.CorrespondingHttpCode:
                    return new NotImplementedException(rawData);
                case BadGatewayException.CorrespondingHttpCode:
                    return new BadGatewayException(rawData);
                case ServiceUnavailableException.CorrespondingHttpCode:
                    return new ServiceUnavailableException(rawData);
                case GatewayTimeoutException.CorrespondingHttpCode:
                    return new GatewayTimeoutException(rawData);
                case HttpVersionNotSupportedException.CorrespondingHttpCode:
                    return new HttpVersionNotSupportedException(rawData);

                default:
                    return new UnknownApiErrorException(rawData, response.StatusCode);
            }
        }
    }
}
