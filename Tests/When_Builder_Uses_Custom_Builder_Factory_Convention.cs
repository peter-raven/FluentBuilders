using System.Linq;
using BuildBuddy.Core;
using FluentAssertions;
using NSubstitute;
using NUnit.Framework;

namespace BuildBuddy.Tests
{
    [TestFixture]
    public class When_Builder_Uses_Custom_Builder_Factory_Convention
    {
        [Test]
        public void The_Supplied_Factory_Is_Asked_For_Builder_Instance_Of_Nested_Builder()
        {
            var factory = Substitute.For<IBuilderFactory>();
            factory.Create<BuilderForTesting>().Returns(new BuilderForTesting());
            var builder = new BuilderForTesting();
            
            builder.BuilderFactoryConvention.UseFactory(factory);
            builder
                .WithReferenceProp()
                .Create();

            factory.Received().Create<BuilderForTesting>();
        }

        [Test]
        public void BuildUsing_Will_Use_The_Custom_Factory()
        {
            var factory = Substitute.For<IBuilderFactory>();
            factory.Create<BuilderForTesting>().Returns(new BuilderForTesting());
            var builder = new BuilderForTesting();

            builder.BuildUsing<BuilderForTesting>();

            builder.Should().BeOfType<BuilderForTesting>();
        }
    }
}