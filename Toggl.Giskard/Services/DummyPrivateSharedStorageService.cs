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

        public void SaveUserId(long userId)
        {
            throw new NotImplementedException();
        }

        public void ClearAll()
        {
            throw new NotImplementedException();
        }
    }
}
