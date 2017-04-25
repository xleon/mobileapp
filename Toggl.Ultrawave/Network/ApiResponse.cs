using Toggl.Multivac;

namespace Toggl.Ultrawave.Network
{
    internal sealed class ApiResponse<T> : IApiResponse<T> 
    {
        public Either<T, ErrorMessage> Data { get; }

        public bool Success { get; }

        public ApiResponse(T data)
        {
            Ensure.ArgumentIsNotNull(data, nameof(data));

            Success = true;
            Data = Either<T, ErrorMessage>.WithLeft(data);
        }

        public ApiResponse(ErrorMessage errorMessage)
        {
            Success = false;
            Data = Either<T, ErrorMessage>.WithRight(errorMessage);
        }

        public static IApiResponse<T> FromErrorMessage(ErrorMessage errorMessage)
			=> new ApiResponse<T>(errorMessage);
    }

    internal static class ApiResponse
    {
        public static IApiResponse<T> FromData<T>(T data)
            => new ApiResponse<T>(data);
   }
}
