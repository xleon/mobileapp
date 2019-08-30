using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using Foundation;
using MultipeerConnectivity;
using Toggl.iOS.Models;
using Toggl.Shared.Extensions;
using Toggl.Shared.Models;
using UIKit;

namespace Toggl.iOS.Services
{
    public class MPCManager: NSObject, IMCNearbyServiceAdvertiserDelegate, IMCNearbyServiceBrowserDelegate
    {
        private ITimeEntry timeEntryToShare;
        public ITimeEntry TimeEntryToShare
        {
            get => timeEntryToShare;
            set
            {
                timeEntryToShare = value;
                foreach (var device in Devices)
                {
                    device.TimeEntryToShare = timeEntryToShare;
                }
            }
        }
        private MCNearbyServiceAdvertiser advertiser;
        private MCNearbyServiceBrowser browser;

        public MCPeerID LocalPeerID;
        private string serviceType = "Toggl-Share";

        public IObservable<(string, DateTimeOffset)> SharedTimeEntryObservable;
        private ISubject<(string, DateTimeOffset)> sharedTimeEntrySubject = new Subject<(string, DateTimeOffset)>();

        public Device[] Devices = {};

        private static MPCManager instance = new MPCManager();
        public static MPCManager Instance => instance;

        private MPCManager()
        {
            var data = NSUserDefaults.StandardUserDefaults.DataForKey("peerID");

            if (data != null)
            {
                var id = (MCPeerID)NSKeyedUnarchiver.UnarchiveObject(data);
                LocalPeerID = id;
            }
            else
            {
                var peerID = new MCPeerID(myDisplayName: UIDevice.CurrentDevice.Name);
                var peerIdData = NSKeyedArchiver.ArchivedDataWithRootObject(peerID);
                NSUserDefaults.StandardUserDefaults.SetValueForKey(peerIdData, (NSString)"peerID");
                LocalPeerID = peerID;
            }

            base.Init();

            advertiser = new MCNearbyServiceAdvertiser(LocalPeerID, null, serviceType);
            advertiser.Delegate = this;

            browser = new MCNearbyServiceBrowser(LocalPeerID, serviceType);
            browser.Delegate = this;

            SharedTimeEntryObservable = sharedTimeEntrySubject.AsObservable().Share();
        }

        public Device DeviceForID(MCPeerID id)
        {
            foreach (var device in Devices)
            {
                if (device.PeerID == id) return device;
            }

            var newDevice = new Device(id);
            newDevice.TimeEntryToShare = timeEntryToShare;
            newDevice.SharedTimeEntryObservable.Subscribe(sharedTimeEntrySubject).Dispose();

            Devices.Append(newDevice);
            return newDevice;
        }

        public void StartAdvertising()
        {
            advertiser.StartAdvertisingPeer();
        }

        public void StartBrowsing()
        {
            browser.StartBrowsingForPeers();
        }

        public void StopAdvertising()
        {
            advertiser.StopAdvertisingPeer();
        }

        public void StopBrowsing()
        {
            browser.StopBrowsingForPeers();
        }

        public List<Device> connectedDevices()
            => Devices
                .Where(device => device.State == MCSessionState.Connected)
                .ToList();

        public void DidReceiveInvitationFromPeer(MCNearbyServiceAdvertiser advertiser, MCPeerID peerID, NSData context,
            MCNearbyServiceAdvertiserInvitationHandler invitationHandler)
        {
            var device = Instance.DeviceForID(peerID);
            device.Connect();
            invitationHandler(true, device.Session);
        }

        public void FoundPeer(MCNearbyServiceBrowser browser, MCPeerID peerID, NSDictionary info)
        {
            var device = Instance.DeviceForID(peerID);
            device.Invite(browser);
        }

        public void LostPeer(MCNearbyServiceBrowser browser, MCPeerID peerID)
        {
            var device = Instance.DeviceForID(peerID);
            device.Disconnect();
        }

        public void CleanUpConnections()
        {
            foreach (var device in Devices)
            {
                device.Disconnect();
            }
        }
    }
}
