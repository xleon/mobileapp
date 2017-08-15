namespace Toggl.Foundation.MvvmCross.Parameters
{
    public sealed class IdParameter
    {
        public int Id { get; set; }

        public static IdParameter WithId(int id) => new IdParameter
        {
            Id = id
        };
    }
}
