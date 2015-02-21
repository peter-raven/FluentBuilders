using BuildBuddy.Core;
using FluentAssertions;
using NUnit.Framework;

namespace BuildBuddy.Tests
{
    [TestFixture]
    public class When_A_BuilderTuple_Is_Instantiated
    {
        [Test]
        public void Giving_A_Builder_To_The_Constructor_Leaves_Instance_Property_Null()
        {
            var t = new BuilderTuple<ExampleClass, ExampleBuilderMock>(new ExampleBuilderMock());
            t.Instance.Should().BeNull("builder tuple should only contain either a builder or an instance");
        }

        [Test]
        public void Giving_A_Builder_To_The_Constructor_Resolves_Using_That_Builder()
        {
            var myBuilder = new ExampleBuilderMock();
            var t = new BuilderTuple<ExampleClass, ExampleBuilderMock>(myBuilder);

            t.Resolve();

            myBuilder.WasAskedToConstruct.Should().BeTrue("the supplied builder should be invoked on resolve");
        }

        [Test]
        public void Giving_A_Builder_To_The_Constructor_Resolves_Using_That_Builder_With_Custom_Setup()
        {
            var myBuilder = new ExampleBuilderMock();
            var t = new BuilderTuple<ExampleClass, ExampleBuilderMock>(myBuilder);

            t.Resolve(x => x.WithSomething("something"));

            myBuilder.Something.Should().Be("something", "the custom setup should be applied to the builder");
        }

        [Test]
        public void Giving_An_Instance_To_The_Constructor_Leaves_Builder_Property_Null()
        {
            var t = new BuilderTuple<ExampleClass, ExampleBuilderMock>(new ExampleClass());
            t.Builder.Should().BeNull("builder tuple should only contain either a builder or an instance");
        }

        [Test]
        public void Giving_An_Instance_To_The_Constructor_Resolves_Using_That_Instance()
        {
            var myClass = new ExampleClass();
            var t = new BuilderTuple<ExampleClass, ExampleBuilderMock>(myClass);
            t.Resolve().Should().BeSameAs(myClass);
        }
    }
}