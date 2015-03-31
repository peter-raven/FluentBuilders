using System.Linq;
using FluentAssertions;
using FluentBuilders.Core;
using NSubstitute;
using NUnit.Framework;

namespace FluentBuilders.Tests
{
    [TestFixture]
    public class When_Builder_Uses_Custom_Builder_Factory_Convention
    {
        [Test]
        public void The_Supplied_Factory_Is_Asked_For_Builder_Instance_Of_Nested_Builder()
        {
            var factory = Substitute.For<IBuilderFactory>();
            factory.Create<ExampleClassBuilder>().Returns(new ExampleClassBuilder());
            var builder = new ExampleClassBuilder();
            
            builder.BuilderFactoryConvention.UseFactory(factory);
            builder
                .WithReferenceProp()
                .Create();

            factory.Received().Create<ExampleClassBuilder>();
        }

        [Test]
        public void BuildUsing_Will_Use_The_Custom_Factory()
        {
            var factory = Substitute.For<IBuilderFactory>();
            factory.Create<ExampleClassBuilder>().Returns(new ExampleClassBuilder());
            var builder = new ExampleClassBuilder();

            builder.BuildUsing<ExampleClassBuilder>();

            builder.Should().BeOfType<ExampleClassBuilder>();
        }
    }
}