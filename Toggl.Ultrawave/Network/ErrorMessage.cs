using System;

namespace Toggl.Ultrawave.Network
{
    public struct ErrorMessage
    {
        private readonly string value;

        private ErrorMessage(string value)
        {
            this.value = value;
        }
        public override string ToString() => value;

        public static implicit operator ErrorMessage(string value)
        {
            if (value != null) return new ErrorMessage(value);
            throw new InvalidCastException();
        }
        public static implicit operator string(ErrorMessage value)
            => value.ToString();
    }
}
