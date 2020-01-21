using System;

namespace Toggl.Core.UI.ViewModels.MainLog.Identity
{
    internal sealed class DayHeaderKey : IMainLogKey
    {
        private readonly DateTime date;

        public DayHeaderKey(DateTime date)
        {
            this.date = date;
        }

        public long Identifier()
            => date.ToBinary();

        public bool Equals(IMainLogKey other)
            => other is DayHeaderKey headerKey && date == headerKey.date;

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            return obj is DayHeaderKey other && Equals(other);
        }

        public override int GetHashCode()
        {
            return date.GetHashCode();
        }
    }
}
