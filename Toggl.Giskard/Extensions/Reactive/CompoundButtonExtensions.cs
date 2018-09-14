using System;
using Android.Widget;
using Toggl.Foundation.MvvmCross.Reactive;

namespace Toggl.Giskard.Extensions.Reactive
{
    public static class CompoundButtonExtensions
    {
        public static Action<bool> Checked(this IReactive<CompoundButton> reactive)
            => isChecked => reactive.Base.Checked = isChecked;
    }
}
