using System;
using FluentAssertions;
using Xunit;

namespace Toggl.Ultrawave.Tests
{
    public abstract class BaseModelTests<T>
        where T : new()
    {
        protected abstract string ValidJson { get; }

        protected abstract T ValidObject { get; }

        [Fact]
        public void HasConstructorWhichCopiesValuesFromInterfaceToTheNewInstance()
        {
            var clonedObject = Activator.CreateInstance(typeof(T), ValidObject);

            clonedObject.Should().NotBeSameAs(ValidObject);
            clonedObject.ShouldBeEquivalentTo(ValidObject, options => options.IncludingProperties());
        }

        [Fact]
        public void CanBeDeserialized()
        {
            SerializationHelper.CanBeDeserialized(ValidJson, ValidObject);
        }

        [Fact]
        public void CanBeSerialized()
        {
            SerializationHelper.CanBeSerialized(ValidJson, ValidObject);
        }
    }
}
