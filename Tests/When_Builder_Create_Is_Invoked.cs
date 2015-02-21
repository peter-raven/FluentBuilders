using BuildBuddy.Core;
using FluentAssertions;
using NUnit.Framework;

namespace BuildBuddy.Tests
{
    [TestFixture]
    public class When_Builder_Create_Is_Invoked
    {
        [Test]
        public void The_Concrete_Builder_Is_Asked_To_Construct()
        {
            var manager = new SimpleBuilderManager();
            var builder = manager.BuildUsing<ExampleBuilderMock>();

            builder.Create();

            builder.WasAskedToConstruct.Should().BeTrue();
        }

        [Test]
        public void The_Concrete_Builder_Is_Asked_To_Construct_With_The_Given_Seed()
        {
            var manager = new SimpleBuilderManager();
            var builder = manager.BuildUsing<ExampleBuilderMock>();

            builder.Create(42);

            builder.WasAskedToConstructWithSeeds.Should().Contain(42);
        }

        [Test]
        public void The_Created_Instance_Matches_The_Factory_Generic()
        {
            var manager = new SimpleBuilderManager();
            var builder = manager.BuildUsing<ExampleBuilderMock>();

            object instance = builder.Create();

            instance.Should().BeOfType<ExampleClass>();
        }


    }
}