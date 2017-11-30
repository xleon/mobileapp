using System;

namespace Toggl.Foundation.MvvmCross.Services
{
    public interface IApiErrorHandlingService
    {
        bool TryHandleDeprecationError(Exception error);
        bool TryHandleUnauthorizedError(Exception error);
    }
}
