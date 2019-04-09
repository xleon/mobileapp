namespace Toggl.Core.Diagnostics
{
    public interface IStopwatch
    {
        void Start();

        void Stop();

        MeasuredOperation Operation { get; }
    }
}
