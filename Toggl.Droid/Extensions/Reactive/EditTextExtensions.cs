using System;
using System.Reactive;
using System.Reactive.Linq;
using Android.Widget;
using Toggl.Core.MvvmCross.Reactive;
using Toggl.Shared.Extensions;

namespace Toggl.Giskard.Extensions.Reactive
{
    public static class EditTextExtensions
    {
        public static IObservable<Unit> EditorActionSent(this IReactive<EditText> reactive)
            => Observable
                .FromEventPattern<TextView.EditorActionEventArgs>(e => reactive.Base.EditorAction += e, e => reactive.Base.EditorAction -= e)
                .SelectUnit();
    }
}
