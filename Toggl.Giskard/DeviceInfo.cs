using System;
using Toggl.Foundation;

namespace Toggl.Giskard
{
    public sealed class DeviceInfo : IDeviceInfo
    {
        public string PhoneModel => throw new NotImplementedException();

        public string OperatingSystem => throw new NotImplementedException();
    }
}
