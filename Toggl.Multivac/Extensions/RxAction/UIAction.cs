using System;
using System.Reactive;

namespace Toggl.Multivac.Extensions
{
    public class UIAction : RxAction<Unit, Unit>
    {
        public UIAction(Func<IObservable<Unit>> workFactory) : base(_ => workFactory())
        {
        }

        public UIAction(Func<IObservable<Unit>> workFactory, IObservable<bool> enabledIf) : base(_ => workFactory(), enabledIf)
        {
        }

        public IObservable<Unit> Execute()
            => Execute(Unit.Default);
    }

    public static class RxActionExtensions
    {
        public static IObservable<Unit> Execute(this UIAction action)
        {
            return action.Execute(Unit.Default);
        }
    }
}
