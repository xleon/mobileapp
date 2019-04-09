using System;
using Toggl.Core.MvvmCross.Reactive;
using UIKit;

namespace Toggl.Daneel.Extensions.Reactive
{
    public static class UIControlExtensions
    {
        public static Action<bool> Enabled(this IReactive<UIControl> reactive)
            => enabled => reactive.Base.Enabled = enabled;
    }
}
