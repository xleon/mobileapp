using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FluentAssertions;
using Toggl.Foundation.Analytics;
using Xunit;
using static System.Reflection.BindingFlags;
using Toggl.Multivac.Extensions;

namespace Toggl.Foundation.Tests.AnalyticsService
{
    public sealed class ComplexEventsTrackingTests
    {
        public sealed class TheToDictionaryMethod
        {
            [Fact]
            public void SerializesAllPropertiesToDictionary()
            {
                var trackableEventType = typeof(ITrackableEvent);

                trackableEventType.Assembly
                    .GetTypes()
                    .Where(trackableEventType.IsAssignableFrom)
                    .Where(type => type.IsClass)
                    .ForEach(ensureTypeIsCorrectlySerializedToDictionary);
            }

            private void ensureTypeIsCorrectlySerializedToDictionary(Type type)
            {
                var properties = type.GetProperties()
                    .Where(prop => prop.Name != nameof(ITrackableEvent.EventName))
                    .ToList();

                var instance = createInstance(type);

                validateInstance(type, properties, instance);
            }

            private ITrackableEvent createInstance(Type type)
            {
                var constructor = type
                    .GetConstructors(Instance | Public | NonPublic)
                    .First();
                var parameterCount = constructor.GetParameters().Length;
                var defaultParameters = new object[parameterCount];
                return (ITrackableEvent)constructor.Invoke(defaultParameters);
            }

            private void validateInstance(Type type, List<PropertyInfo> properties, ITrackableEvent trackableEvent)
            {
                var toDictionaryMethod = type.GetMethod(nameof(ITrackableEvent.ToDictionary));

                if (toDictionaryMethod == null)
                    throw new InvalidOperationException("ToDictionary method is missing.");

                var dictionary = (Dictionary<string, string>)toDictionaryMethod.Invoke(trackableEvent, null);

                var reason = "all of the object's properties must be serialized into a dictionary.";
                dictionary.Count.Should().Be(properties.Count, reason);
                dictionary.Should().ContainKeys(properties.Select(prop => prop.Name), reason);
            }
        }
    }
}
