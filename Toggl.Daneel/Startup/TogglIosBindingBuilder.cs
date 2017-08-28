using MvvmCross.Binding.Combiners;
using MvvmCross.Binding.iOS;

namespace Toggl.Daneel.Startup
{
    public sealed class TogglIosBindingBuilder : MvxIosBindingBuilder
    {
        protected override void FillValueCombiners(IMvxValueCombinerRegistry registry)
        {
            base.FillValueCombiners(registry);

            registry.AddOrOverwrite("ProjectTaskClient", new ProjectTaskClientValueCombiner());
        }
    }
}
