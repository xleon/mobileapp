using WatchConnectivity;

namespace Toggl.iOS.Services
{
    public sealed class WatchService : WCSessionDelegate
    {
        public void TryLogWatchConnectivity()
        {
            if (WCSession.IsSupported)
            {
                var session = WCSession.DefaultSession;
                session.Delegate = this;
                session.ActivateSession();
                IosDependencyContainer.Instance.AnalyticsService.WatchPaired.Track(session.Paired);
            }
        }

        public override void ActivationDidComplete(WCSession session, WCSessionActivationState activationState, Foundation.NSError error)
        {
        }

        public override void DidBecomeInactive(WCSession session)
        {
        }

        public override void DidDeactivate(WCSession session)
        {
        }
    }
}
