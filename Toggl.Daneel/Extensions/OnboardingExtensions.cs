using System;
using CoreGraphics;
using Toggl.PrimeRadiant.Onboarding;
using UIKit;
using Toggl.Foundation.MvvmCross.Helper;
using Toggl.PrimeRadiant.Settings;
using Toggl.PrimeRadiant.Extensions;
using System.Reactive.Linq;

namespace Toggl.Daneel.Extensions
{
    public static class Onboarding
    {
        private static readonly nfloat cellRadius = 8;

        private static readonly CGSize shadowOffset = new CGSize(0, 2);

        private static readonly CGColor shadowColor = UIColor.Black.CGColor;

        private static readonly nfloat shadowRadius = 6;

        private const float shadowOpacity = 0.1f;

        private static readonly TimeSpan visibilityDelay = TimeSpan.FromMilliseconds(600);

        public static void MockSuggestion(this UIView view)
        {
            view.Layer.CornerRadius = cellRadius;
            view.Layer.ShadowOffset = shadowOffset;
            view.Layer.ShadowColor = shadowColor;
            view.Layer.ShadowOpacity = shadowOpacity;
            view.Layer.ShadowRadius = shadowRadius;
        }

        public static IDisposable ManageVisibilityOf(this IOnboardingStep step, UIView view)
        {
            view.Hidden = true;

            void toggleVisibilityOnMainThread(bool shouldBeVisible)
            {
                UIApplication.SharedApplication.InvokeOnMainThread(
                    () => toggleVisiblity(shouldBeVisible));
            }

            void toggleVisiblity(bool shouldBeVisible)
            {
                var isVisible = view.Hidden == false;
                if (isVisible == shouldBeVisible) return;

                if (shouldBeVisible)
                {
                    view.Hidden = false;
                    view.Alpha = 0;
                    view.Transform = CGAffineTransform.MakeScale(0.01f, 0.01f);
                    AnimationExtensions.Animate(
                        Animation.Timings.LeaveTiming,
                        Animation.Curves.Bounce,
                        () =>
                        {
                            view.Alpha = 1;
                            view.Transform = CGAffineTransform.MakeScale(1f, 1f);
                        },
                        () =>
                        {
                            isVisible = true;
                        });
                }
                else
                {
                    view.Alpha = 1;
                    view.Transform = CGAffineTransform.MakeScale(1f, 1f);
                    AnimationExtensions.Animate(
                        Animation.Timings.LeaveTiming,
                        Animation.Curves.Bounce,
                        () =>
                        {
                            view.Alpha = 0;
                            view.Transform = CGAffineTransform.MakeScale(0.01f, 0.01f);
                        },
                        () =>
                        {
                            view.Hidden = true;
                            isVisible = false;
                        });
                }
            }

            return step.ShouldBeVisible
                       .Delay(visibilityDelay)
                       .Subscribe(toggleVisibilityOnMainThread);
        }

        public static void DismissByTapping(this IDismissable step, UIView view)
        {
            var tapGestureRecognizer = new UITapGestureRecognizer(() => step.Dismiss());
            view.AddGestureRecognizer(tapGestureRecognizer);
        }

        public static IDisposable ManageDismissableTooltip(this IOnboardingStep step, UIView tooltip, IOnboardingStorage storage)
        {
            var dismissableStep = step.ToDismissable(step.GetType().FullName, storage);
            dismissableStep.DismissByTapping(tooltip);
            return dismissableStep.ManageVisibilityOf(tooltip);
        }
    }
}
