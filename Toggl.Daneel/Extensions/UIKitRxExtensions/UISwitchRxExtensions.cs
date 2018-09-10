using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using UIKit;

namespace Toggl.Daneel.Extensions
{
    public static partial class UIKitRxExtensions
    {
        public static IObservable<Unit> Changed(this UISwitch uiSwitch)
            => Observable.Create<Unit>(observer =>
            {
                void changed(object sender, EventArgs args)
                {
                    observer.OnNext(Unit.Default);
                }

                uiSwitch.ValueChanged += changed;

                return Disposable.Create(() => uiSwitch.ValueChanged -= changed);
            });
    }
}
