namespace Toggl.PrimeRadiant.Exceptions
{
    public class EntityAlreadyExistsException : DatabaseException
    {
        private const string defaultErrorMessage = "Entity already exists in database";

        public EntityAlreadyExistsException()
            : this(defaultErrorMessage)
        {
        }

        public EntityAlreadyExistsException(string message)
            : base(message)
        {
        }
    }
}
