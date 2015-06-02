using System;
using FluentAssertions;
using NUnit.Framework;

namespace FluentBuilders.Tests
{
    [TestFixture]
    public class When_Using_Nested_Builder
    {
        [Test]
        public void Nested_Lambda_Is_Applied_On_Nested_Builder()
        {
            var builder = new ExampleClassBuilder()
                .WithReferenceProp(x => x.WithStringProp("I am child"));

            ExampleClass res = builder.Create();

            res.ReferenceProp.StringProp.Should().Be("I am child");
        }

        [Test]
        public void Nested_Lambda_Is_Merged_With_Default_Lambdas()
        {
            var builder = new ExampleClassBuilder()
                .WithReferenceProp(x => x.WithDateTimeProp(DateTime.Now));

            ExampleClass res = builder.Create();

            res.ReferenceProp.StringProp.Should().Be("I am default");
        } 
    }
}