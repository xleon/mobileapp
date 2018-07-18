using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using Toggl.Foundation.MvvmCross.Helper;
using UIKit;

namespace Toggl.Daneel.Extensions
{
    public static partial class UIKitRxExtensions
    {
        public static IObservable<Unit> Tapped(this UIView view)
            => Observable.Create<Unit>(observer =>
            {
                var gestureRecognizer = new UITapGestureRecognizer(() => observer.OnNext(Unit.Default));
                gestureRecognizer.ShouldRecognizeSimultaneously = (recognizer, otherRecognizer) => true;
                view.AddGestureRecognizer(gestureRecognizer);

                return Disposable.Create(() => view.RemoveGestureRecognizer(gestureRecognizer));
            });


        public static Action<bool> BindIsVisible(this UIView view)
            => isVisible => view.Hidden = !isVisible;

        public static Action<bool> BindIsVisibleWithFade(this UIView view)
            => isVisible =>
            {
                var alpha = isVisible ? 1 : 0;
                AnimationExtensions.Animate(
                    Animation.Timings.EnterTiming,
                    Animation.Curves.EaseIn,
                    () => view.Alpha = alpha
                );
            };
    }
}
