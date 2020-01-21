using Android.Text;
using Android.Widget;
using AndroidX.ViewPager.Widget;
using Java.Lang;
using System;
using System.Reactive.Linq;
using Toggl.Core.UI.Reactive;
using static Android.Views.View;
using static AndroidX.ViewPager.Widget.ViewPager;

namespace Toggl.Droid.Extensions.Reactive
{
    public static class ViewPagerExtensions
    {
        public static IObservable<int> PageSelected(this IReactive<ViewPager> reactive)
            => Observable
                .FromEventPattern<PageSelectedEventArgs>(e => reactive.Base.PageSelected += e, e => reactive.Base.PageSelected -= e)
                .Select(ev => ev.EventArgs.Position);
    }
}
