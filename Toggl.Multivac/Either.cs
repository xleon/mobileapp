using System;

namespace Toggl.Multivac
{
    public sealed class Either<TLeft, TRight>
    {
        private readonly bool useLeft;
        private readonly TLeft left;
        private readonly TRight right;

        private Either(TLeft left)
        {
            this.left = left;
            useLeft = true;
        }

        private Either(TRight right)
        {
            this.right = right;
            useLeft = false;
        }

        public void Match(Action<TLeft> leftAction, Action<TRight> rightAction)
        {
            Ensure.ArgumentIsNotNull(leftAction, nameof(leftAction));
            Ensure.ArgumentIsNotNull(rightAction, nameof(rightAction));

            if (useLeft)
            {
                leftAction(left);
                return;
            }

            rightAction(right);
        }

        public T Match<T>(Func<TLeft, T> leftAction, Func<TRight, T> rightFunc)
        {
            Ensure.ArgumentIsNotNull(leftAction, nameof(leftAction));
            Ensure.ArgumentIsNotNull(rightFunc, nameof(rightFunc));

            return useLeft ? leftAction(left) : rightFunc(right);
        }

        public static Either<TLeft, TRight> Left(TLeft left)
            => new Either<TLeft, TRight>(left);

        public static Either<TLeft, TRight> Right(TRight right)
            => new Either<TLeft, TRight>(right);
    }
}
