using Android.Support.V7.Widget;
using System;
using System.Reactive;
using System.Reactive.Linq;
using Toggl.Core.UI.Reactive;
using Toggl.Shared.Extensions;

namespace Toggl.Droid.Extensions.Reactive
{
    public static class ToolbarExtensions
    {
        public static IObservable<Unit> NavigationTapped(this IReactive<Toolbar> reactive)
            => Observable
                .FromEventPattern<Toolbar.NavigationClickEventArgs>(
                    e => reactive.Base.NavigationClick += e,
                    e => reactive.Base.NavigationClick -= e)
                .SelectUnit();
    }
}
