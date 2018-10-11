namespace Toggl.Foundation.Diagnostics
{
    public interface IStopwatchFactory
    {
        IStopwatch Create(MeasuredOperation operation, bool outputToConsole = false);
    }
}
