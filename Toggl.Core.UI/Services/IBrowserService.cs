using System;

namespace Toggl.Core.MvvmCross.Services
{
    public interface IBrowserService
    {
        void OpenUrl(string url);

        void OpenStore();
    }
}
