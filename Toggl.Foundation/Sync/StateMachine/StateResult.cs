namespace Toggl.Foundation.Sync
{
    public interface IStateResult { }

    internal sealed class StateResult : IStateResult
    {
        private readonly Transition singletonTransition;

        public StateResult()
        {
            singletonTransition = new Transition(this);
        }

        public Transition Transition() => singletonTransition;
    }

    internal sealed class StateResult<T> : IStateResult
    {
        public Transition<T> Transition(T parameter) => new Transition<T>(this, parameter);
    }
}
