namespace Toggl.PrimeRadiant.Settings
{
    public interface IAccessRestrictionStorage
    {
        void SetClientOutdated();
        void SetApiOutdated();
        void SetUnauthorizedAccess(string apiToken);
        void SetNoWorkspaceStateReached(bool hasNoWorkspace);
        bool IsClientOutdated();
        bool IsApiOutdated();
        bool IsUnauthorized(string apiToken);
        bool HasNoWorkspace();
    }
}
