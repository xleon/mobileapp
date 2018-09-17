namespace Toggl.Foundation.MvvmCross.Reactive
{
    public interface IReactive<out TBase>
    {
        TBase Base { get; }
    }
}
