namespace Toggl.Core.MvvmCross.Reactive
{
    public interface IReactive<out TBase>
    {
        TBase Base { get; }
    }
}
