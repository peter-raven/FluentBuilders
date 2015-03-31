using System.Linq;
using FluentAssertions;
using FluentBuilders.Core;
using NUnit.Framework;

namespace FluentBuilders.Tests
{
    [TestFixture]
    public class When_Builder_Create_Is_Invoked
    {
        [Test]
        public void The_Concrete_Builder_Is_Asked_To_Construct()
        {
            var builder = new BuilderForTesting<ExampleClass>();

            builder.Create();

            builder.WasAskedToConstruct.Should().BeTrue();
        }

        [Test]
        public void The_Concrete_Builder_Is_Asked_To_Construct_With_The_Given_Seed()
        {
            var builder = new BuilderForTesting<ExampleClass>();

            builder.Create(42);

            builder.WasAskedToConstructWithSeeds.Should().Contain(42);
        }

        [Test]
        public void The_Created_Instance_Matches_The_Factory_Generic()
        {
            var builder = new BuilderForTesting<ExampleClass>();

            object instance = builder.Create();

            instance.Should().BeOfType<ExampleClass>();
        }

        [Test]
        public void Customizations_Are_Performed_On_Built_Instance()
        {
            string customString = Generate.RandomString(50);
            ExampleClass result = new BuilderForTesting<ExampleClass>()
                .Customize(b => b.StringProp = customString)
                .Create();

            result.StringProp.Should().Be(customString, "customize was used to set the property.");
        }

        [Test]
        public void Second_Create_Builds_A_Different_Instance()
        {
            var builderForTesting = new BuilderForTesting<ExampleClass>();

            ExampleClass result1 = builderForTesting.Create();
            ExampleClass result2 = builderForTesting.Create();

            result1.Should().NotBeSameAs(result2);
        }

        [Test]
        public void Multiple_Creates_Pass_Seeds_To_Build_Method()
        {
            var builderForTesting = new BuilderForTesting<ExampleClass>();

            builderForTesting.Create(42);
            builderForTesting.Create(1);

            builderForTesting.WasAskedToConstructWithSeeds.Should().ContainInOrder(new[] { 42, 1 });
        }

        [Test]
        public void CreateMany_Builds_Different_Instances()
        {
            var builderForTesting = new BuilderForTesting<ExampleClass>();

            ExampleClass[] results = builderForTesting.CreateMany(3).ToArray();

            results.Should().OnlyHaveUniqueItems();
        }

        [Test]
        public void CreateMany_Pass_Sequental_Seeds_To_Build_Method()
        {
            var builderForTesting = new BuilderForTesting<ExampleClassBuilder>();

            builderForTesting.CreateMany(3);

            builderForTesting.WasAskedToConstructWithSeeds.Should().ContainInOrder(new[] { 0, 1, 2 });
        }
    }
}