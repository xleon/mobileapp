namespace Toggl.Ultrawave.Network
{
    internal struct HttpHeader
    {
        public enum HeaderType
        {
            Auth,
            Other
        }

        public HeaderType Type { get; } 

        public string Name { get; }

        public string Value { get; }

        public HttpHeader(string name, string value, HeaderType type = HeaderType.Other)
        {
            Type = type;
            Name = name;
            Value = value;
        }
    }
}
