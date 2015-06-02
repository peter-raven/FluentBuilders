using System;
using FluentAssertions;
using FluentBuilders.Core;
using NUnit.Framework;

namespace FluentBuilders.Tests
{
    [TestFixture]
    public class When_Builder_Uses_Setup
    {
        [Test]
        public void Setup_Is_Applied_On_Create()
        {
            var builder = new ExampleClassBuilder();

            builder.Setup(x => x.WithStringProp("Foo"));

            ExampleClass res = builder.Create();

            res.StringProp.Should().Be("Foo");
        }

        [Test]
        public void Setup_Overrides_On_Create()
        {
            var builder = new ExampleClassBuilder()
                .WithStringProp("Foo");
            builder.Setup(x => x.WithStringProp("Bar"));

            ExampleClass res = builder.Create();

            res.StringProp.Should().Be("Bar");
        }
    }
}