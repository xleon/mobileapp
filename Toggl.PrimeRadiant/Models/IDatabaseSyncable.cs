namespace Toggl.PrimeRadiant
{
    public interface IDatabaseSyncable
    {
        SyncStatus SyncStatus { get; }
    }
}
