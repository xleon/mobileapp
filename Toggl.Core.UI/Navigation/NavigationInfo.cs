using System;

namespace Toggl.Core.UI.Navigation
{
    public struct NavigationInfo<TPayload>
    {
        public TPayload Payload { get; }

        public Type ViewModelType { get; }

        public NavigationInfo(TPayload payload, Type viewModelType)
        {
            Payload = payload;
            ViewModelType = viewModelType;
        }
    }
}
