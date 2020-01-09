using Android.Animation;
using Android.Views.Animations;
using System;
using Toggl.Shared;
using static System.Math;
using AndroidColor = Android.Graphics.Color;

namespace Toggl.Droid.Views
{
    public sealed partial class DonutChartView
    {
        private class SliceSelectionAnimator
        {
            private Action updateAction;
            private ValueAnimator animator;
            private ArgbEvaluator colorAnimator;

            private float startMidAngle;
            private float midAngleTotalChange;

            private float startSweepAngle;
            private float sweepAngleTotalChange;

            private float direction;
            private AndroidColor destinationColor;

            public float StartAngle { get; private set; }
            public float SweepAngle { get; private set; }

            public AndroidColor Color { get; private set; }

            public SliceSelectionAnimator(Action updateAction)
            {
                this.updateAction = updateAction;

                animator = ValueAnimator.OfFloat(0, 1);
                colorAnimator = new ArgbEvaluator();
                animator.SetDuration(350);
                animator.SetInterpolator(new DecelerateInterpolator());
                animator.Update += onSelectionAnimationUpdate;
            }

            public void StartAnimation(float destinationAngle, float desiredSweepAngle, AndroidColor destinationColor, float originalAngle, float originalSweepAngle)
            {
                var fromMidAngle = (originalAngle + originalSweepAngle / 2).NormalizedAngle();
                var toMidAngle = (destinationAngle + desiredSweepAngle / 2).NormalizedAngle();

                var midAngleDifference = toMidAngle - fromMidAngle;
                var rotateAngle = midAngleDifference.ToRadians();
                direction = Sign(Sin(rotateAngle));

                startMidAngle = fromMidAngle;
                midAngleTotalChange = Abs(midAngleDifference);

                // use smaller of the two angles between mid angles.
                if (midAngleTotalChange > 180)
                    midAngleTotalChange = 360 - midAngleTotalChange;

                startSweepAngle = originalSweepAngle;
                sweepAngleTotalChange = desiredSweepAngle - originalSweepAngle;

                this.destinationColor = destinationColor;

                animator.Start();
            }

            public void ContinueAnimation(float destinationAngle, float desiredSweepAngle, AndroidColor destinationColor)
                => StartAnimation(destinationAngle, desiredSweepAngle, destinationColor, StartAngle, SweepAngle);

            private void onSelectionAnimationUpdate(object sender, ValueAnimator.AnimatorUpdateEventArgs e)
            {
                var value = (float)e.Animation.AnimatedValue;

                Color = new AndroidColor((int)colorAnimator.Evaluate(value, Color.ToArgb(), destinationColor.ToArgb()));

                SweepAngle = startSweepAngle + sweepAngleTotalChange * value;
                var halfSweep = SweepAngle / 2;

                var currentMidAngle = startMidAngle + direction * midAngleTotalChange * value;

                StartAngle = currentMidAngle - halfSweep;

                updateAction();
            }
        }
    }
}
