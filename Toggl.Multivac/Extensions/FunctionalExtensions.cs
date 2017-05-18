using System;
namespace Toggl.Multivac.Extensions
{
    public static class FunctionalExtensions
    {
        public static TResult Apply<T, TResult>(this T self, Func<T, TResult> funcToApply)
            => funcToApply(self);
    }
}
