using System;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace Toggl.Multivac.Extensions
{
    public sealed class UIAction : RxAction<Unit, Unit>
    {
        public UIAction(Func<IObservable<Unit>> workFactory)
            : base(_ => workFactory())
        {
        }

        public UIAction(Func<IObservable<Unit>> workFactory, IObservable<bool> enabledIf)
            : base(_ => workFactory(), enabledIf)
        {
        }

        public IObservable<Unit> Execute()
            => Execute(Unit.Default);

        public static UIAction FromAction(Action action)
        {
            IObservable<Unit> workFactory()
                => Observable.Create<Unit>(observer =>
                {
                    action();
                    observer.CompleteWith(Unit.Default);
                    return Disposable.Empty;
                });

            return new UIAction(workFactory);
        }
    }

    public static class RxActionExtensions
    {
        public static IObservable<Unit> Execute(this UIAction action)
        {
            return action.Execute(Unit.Default);
        }
    }
}
