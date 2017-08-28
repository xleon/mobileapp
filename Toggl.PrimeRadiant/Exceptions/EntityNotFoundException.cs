namespace Toggl.PrimeRadiant.Exceptions
{
    public sealed class EntityNotFoundException : DatabaseException
    {
        private const string defaultErrorMessage = "Entity not found in database";

        public EntityNotFoundException()
            : this(defaultErrorMessage)
        {
        }

        public EntityNotFoundException(string message)
            : base(message)
        {
        }
    }
}
