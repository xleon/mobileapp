using System;
using Toggl.Core.MvvmCross.Reactive;
using UIKit;

namespace Toggl.Daneel.Extensions.Reactive
{
    public static class UIApplicationExtensions
    {
        public static Action<bool> NetworkActivityIndicatorVisible(this IReactive<UIApplication> reactive)
            => visible => reactive.Base.NetworkActivityIndicatorVisible = visible;
    }
}
