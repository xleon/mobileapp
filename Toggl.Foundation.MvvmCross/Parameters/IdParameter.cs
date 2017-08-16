using Toggl.Multivac;

namespace Toggl.Foundation.MvvmCross.Parameters
{
    [Preserve(AllMembers = true)]
    public sealed class IdParameter
    {
        public long Id { get; set; }

        public static IdParameter WithId(long id) => new IdParameter
        {
            Id = id
        };
    }
}
