using System;
using System.Reactive.Linq;
using Toggl.Foundation.MvvmCross.Reactive;
using UIKit;

namespace Toggl.Daneel.Extensions.Reactive
{
    public static class UITextFieldExtensions
    {
        public static IObservable<string> Text(this IReactive<UITextField> reactive)
            => Observable
                .FromEventPattern(handler => reactive.Base.EditingChanged += handler, handler => reactive.Base.EditingChanged -= handler)
                .Select(_ => reactive.Base.Text);

        public static Action<bool> SecureTextEntry(this IReactive<UITextField> reactive) => isSecure =>
        {
            reactive.Base.ResignFirstResponder();
            reactive.Base.SecureTextEntry = isSecure;
            reactive.Base.BecomeFirstResponder();
        };

        public static Action<string> TextObserver(this IReactive<UITextField> reactive)
           => text => reactive.Base.Text = text;
    }
}
