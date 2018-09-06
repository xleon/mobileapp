using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using CoreGraphics;
using Toggl.Daneel.Views;
using Toggl.Foundation.MvvmCross.Helper;
using UIKit;

namespace Toggl.Daneel.Extensions
{
    public static partial class UIKitRxExtensions
    {
        public static Action<bool> BindFadeRight(this FadeView view)
            => useRightFading => view.FadeRight = useRightFading;

        public static Action<bool> BindFadeLeft(this FadeView view)
            => useLeftFading => view.FadeLeft = useLeftFading;
    }
}
