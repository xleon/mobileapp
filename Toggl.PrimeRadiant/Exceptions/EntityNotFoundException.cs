using System;

namespace Toggl.PrimeRadiant.Exceptions
{
    public class EntityNotFoundException : DatabaseException
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

        public EntityNotFoundException(InvalidOperationException ex)
            : base(defaultErrorMessage, ex)
        {
        }
    }
}
