using System;
using Toggl.Foundation.MvvmCross.Reactive;
using UIKit;

namespace Toggl.Daneel.Extensions.Reactive
{
    public static class NSConstraintExtensions
    {
        public static Action<nfloat> Constant(this IReactive<NSLayoutConstraint> reactive)
            => constant => reactive.Base.Constant = constant;

        public static Action<bool> Active(this IReactive<NSLayoutConstraint> reactive)
            => isActive => reactive.Base.Active = isActive;
    }
}
