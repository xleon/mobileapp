using System;
using Toggl.Foundation.MvvmCross.Reactive;
using UIKit;

namespace Toggl.Daneel.Extensions.Reactive
{
    public static class UIApplicationExtensions
    {
        public static Action<bool> NetworkActivityIndicatorVisible(this IReactive<UIApplication> reactive)
            => visible => reactive.Base.NetworkActivityIndicatorVisible = visible;
    }
}
