namespace Toggl.Foundation.Services
{
    public interface IPrivateSharedStorageService
    {
        void SaveApiToken(string apiToken);
        void ClearAll();
    }
}
