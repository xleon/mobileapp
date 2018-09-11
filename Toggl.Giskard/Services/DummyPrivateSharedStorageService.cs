using System;
using Toggl.Foundation.Services;

namespace Toggl.Giskard.Services
{
    public class DummyPrivateSharedStorageService : IPrivateSharedStorageService
    {
        public void SaveApiToken(string apiToken)
        {
            throw new NotImplementedException();
        }

        void ClearAll()
        {

        }
    }
}
