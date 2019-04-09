using System;

namespace Toggl.Core.UI.Services
{
    public interface IBrowserService
    {
        void OpenUrl(string url);

        void OpenStore();
    }
}
