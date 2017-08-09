namespace Toggl.PrimeRadiant
{
    public interface IDatabaseSyncable
    {
        bool IsDirty { get; }
    }
}
