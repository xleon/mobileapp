namespace Toggl.Core.MvvmCross.Helper
{
    public enum TemporalInconsistency
    {
        StartTimeAfterCurrentTime,
        StartTimeAfterStopTime,
        StopTimeBeforeStartTime,
        DurationTooLong
    }
}
