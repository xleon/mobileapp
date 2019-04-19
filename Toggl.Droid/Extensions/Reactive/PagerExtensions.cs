using System;
using System.Reactive;
using System.Reactive.Linq;
using Android.Support.V4.View;
using Android.Views;
using Toggl.Core.UI.Reactive;
using Toggl.Shared.Extensions;

namespace Toggl.Droid.Extensions.Reactive
{
    public static class PagerExtensions
    {
        public static IObservable<int> CurrentItem(this IReactive<ViewPager> reactive)
            => Observable
                .FromEventPattern<ViewPager.PageScrolledEventArgs>(e => reactive.Base.PageScrolled += e, e => reactive.Base.PageScrolled -= e)
                .Select(_ => reactive.Base.CurrentItem);
    }
}
