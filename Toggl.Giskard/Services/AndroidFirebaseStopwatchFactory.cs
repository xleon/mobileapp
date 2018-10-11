using Toggl.Foundation.Diagnostics;

namespace Toggl.Giskard.Services
{
    public sealed class AndroidFirebaseStopwatchFactory : BaseStopwatchFactory
    {
        protected override IStopwatch NativeCreate(MeasuredOperation operation)
            => new AndroidFirebaseStopwatch(operation);

        private class AndroidFirebaseStopwatch : IStopwatch
        {
            //private Trace firebaseTrace;

            public MeasuredOperation Operation { get; }

            public AndroidFirebaseStopwatch(MeasuredOperation operation)
            {
                Operation = operation;

                //firebaseTrace = FirebasePerformance.Instance.NewTrace(operation.ToString());
            }

            public void Start()
            {
                //firebaseTrace.Start();
            }

            public void Stop()
            {
                //firebaseTrace.Stop();
            }
        }
    }
}
