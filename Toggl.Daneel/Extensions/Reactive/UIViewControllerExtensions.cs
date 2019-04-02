using System;
using CoreGraphics;
using Toggl.Foundation.MvvmCross.Reactive;
using UIKit;

namespace Toggl.Daneel.Extensions.Reactive
{
    public static class UIViewControllerExtensions
    {
        public static Action<CGSize> PreferredContentSize(this IReactive<UIViewController> reactive)
            => preferredContentSize => reactive.Base.PreferredContentSize = preferredContentSize;
    }
}
