using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Foundation;
using MultipeerConnectivity;
using Toggl.iOS.Services;
using Toggl.Shared.Models;

namespace Toggl.iOS.Models
{
    public class Device : NSObject, IMCSessionDelegate
    {
        public ITimeEntry TimeEntryToShare;
        public MCPeerID PeerID;
        public MCSession Session;
        public MCSessionState State = MCSessionState.NotConnected;
        public IObservable<(string, DateTimeOffset)> SharedTimeEntryObservable;
        private ISubject<(string, DateTimeOffset)> sharedTimeEntrySubject = new Subject<(string, DateTimeOffset)>();

        public Device(MCPeerID peerID)
        {
            this.PeerID = peerID;
            SharedTimeEntryObservable = sharedTimeEntrySubject.AsObservable();

            base.Init();
        }

        public void Connect()
        {
            if (Session != null) return;
            Session = new MCSession(MPCManager.Instance.LocalPeerID);
            Session.Delegate = this;
        }

        public void Disconnect()
        {
            Session?.Disconnect();
            Session = null;
        }

        public void Invite(MCNearbyServiceBrowser browser)
        {
            Connect();
            browser.InvitePeer(PeerID, Session, null, timeout: 10);
        }

        public void Send(ITimeEntry timeEntry)
        {
            var nstimeentry = new NSTimeEntry(timeEntry.Start, timeEntry.Description);
            var payload = NSKeyedArchiver.ArchivedDataWithRootObject(nstimeentry);
            Session.SendData(payload, new[] {PeerID}, MCSessionSendDataMode.Reliable, out var error);
        }

        //delegate
        void IMCSessionDelegate.DidChangeState(MCSession session, MCPeerID peerID, MCSessionState state)
        {
            State = state;

            if (TimeEntryToShare != null && state == MCSessionState.Connected)
            {
                Send(TimeEntryToShare);
            }
        }

        void IMCSessionDelegate.DidReceiveData(MCSession session, NSData data, MCPeerID peerID)
        {
            var nstimeEntry = (NSTimeEntry)NSKeyedUnarchiver.UnarchiveObject(data);
            Debug.WriteLine(nstimeEntry.Start);
            Debug.WriteLine(nstimeEntry.Description);
            Debug.WriteLine("got data");
        }

        void IMCSessionDelegate.DidStartReceivingResource(MCSession session, string resourceName, MCPeerID fromPeer, NSProgress progress) { }

        void IMCSessionDelegate.DidFinishReceivingResource(MCSession session, string resourceName, MCPeerID fromPeer, NSUrl localUrl,
            NSError error) { }

        void IMCSessionDelegate.DidReceiveStream(MCSession session, NSInputStream stream, string streamName, MCPeerID peerID) { }
    }
}
