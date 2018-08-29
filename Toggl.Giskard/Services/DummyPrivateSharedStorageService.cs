using System;
using Toggl.Foundation.Services;

namespace Toggl.Giskard.Services
{
    public class DummyPrivateSharedStorageService : IPrivateSharedStorageService
    {
        public DummyPrivateSharedStorageService()
        {
        }

        void SaveApiToken(string apiToken)
        {
            
        }
    }
}
