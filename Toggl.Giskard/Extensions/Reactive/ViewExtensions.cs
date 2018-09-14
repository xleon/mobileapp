using System;
using System.Reactive;
using System.Reactive.Linq;
using Android.Views;
using Toggl.Foundation.MvvmCross.Reactive;
using Toggl.Multivac.Extensions;

namespace Toggl.Giskard.Extensions.Reactive
{
    public static class ViewExtensions
    {
        public static IObservable<Unit> Tap(this IReactive<View> reactive)
            => Observable
                .FromEventPattern(e => reactive.Base.Click += e, e => reactive.Base.Click -= e)
                .SelectUnit();

        public static Action<bool> Enabled(this IReactive<View> reactive)
            => enabled => reactive.Base.Enabled = enabled;

        public static Action<bool> IsVisible(this IReactive<View> reactive, bool useGone = true)
            => isVisible => reactive.Base.Visibility = isVisible.ToVisibility(useGone);
    }
}
