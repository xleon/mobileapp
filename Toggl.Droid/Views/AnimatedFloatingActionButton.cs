using System;
using Android.Animation;
using Android.Content;
using Android.Graphics;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Google.Android.Material.FloatingActionButton;
using Toggl.Droid.Extensions;

namespace Toggl.Droid.Views
{
    [Register("toggl.droid.views.AnimatedFloatingActionButton")]
    public class AnimatedFloatingActionButton : FloatingActionButton
    {
        private const int animationStrokeWidthDp = 6;
        private const int animationPaddingDp = 1;
        private readonly int animationPaddingPx;
        private const int minAnimationDurationMs = 500;
        private const long animationStartDelayMs = 25;
        private const float fullCircle = 360;
        private const float arcAngleStart = 180;
        private const float arcAngleEnd = fullCircle + arcAngleStart;

        private readonly Paint paint;
        private readonly RectF drawingRect = new RectF(0,0,0,0);
        private readonly ValueAnimator arcAnimator = ValueAnimator.OfFloat(0, 1);
        private float sweepAngle = 0;

        public AnimatedFloatingActionButton(IntPtr javaReference, JniHandleOwnership transfer)
            : base(javaReference, transfer)
        {
        }

        public AnimatedFloatingActionButton(Context context, IAttributeSet attrs)
            : this(context, attrs, 0)
        {
        }

        public AnimatedFloatingActionButton(Context context, IAttributeSet attrs, int defStyle)
            : base(context, attrs, defStyle)
        {
            paint = new Paint { Flags = PaintFlags.AntiAlias };
            paint.SetStyle(Paint.Style.Stroke);
            paint.StrokeWidth = animationStrokeWidthDp.DpToPixels(context);
            paint.Color = context.SafeGetColor(Resource.Color.startTimeEntryButtonAnimation);

            arcAnimator.SetDuration(Math.Max(ViewConfiguration.LongPressTimeout, minAnimationDurationMs));
            arcAnimator.StartDelay = animationStartDelayMs;
            arcAnimator.Update += (_, args) => updateAngle((float)args.Animation.AnimatedValue * arcAngleEnd);

            animationPaddingPx = animationPaddingDp.DpToPixels(context);
        }

        public void TryStartAnimation()
        {
            if(!arcAnimator.IsRunning)
                arcAnimator.Start();
        }

        public void StopAnimation()
        {
            arcAnimator.End();
            updateAngle(0);
        }

        private void updateAngle(float newAngle)
        {
            sweepAngle = newAngle;
            Invalidate();
        }

        protected override void OnSizeChanged(int w, int h, int oldw, int oldh)
        {
            base.OnSizeChanged(w, h, oldw, oldh);

            drawingRect.Top = -animationPaddingPx;
            drawingRect.Left = -animationPaddingPx;

            drawingRect.Right = w + animationPaddingPx;
            drawingRect.Bottom = h + animationPaddingPx;
        }

        public override void Draw(Canvas canvas)
        {
            base.Draw(canvas);
            canvas.DrawArc(drawingRect, arcAngleStart, sweepAngle, false, paint);
        }
    }
}
