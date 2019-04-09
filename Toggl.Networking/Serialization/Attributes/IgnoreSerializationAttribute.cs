using System;

namespace Toggl.Ultrawave.Serialization
{
    [AttributeUsage(AttributeTargets.Property)]
    internal class IgnoreSerializationAttribute : Attribute
    {
    } 
}
