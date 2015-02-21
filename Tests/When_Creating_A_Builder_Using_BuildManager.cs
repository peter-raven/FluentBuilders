using System.Linq;
using BuildBuddy.Core;
using FluentAssertions;
using Microsoft.Practices.ServiceLocation;
using NSubstitute;
using NUnit.Framework;

namespace BuildBuddy.Tests
{


    [TestFixture]
    public class When_Creating_A_Builder_Using_BuildManager
    {
        [Test]
        public void The_Supplied_Service_Locator_Is_Asked_For_Factory_Instance()
        {
            var serviceLocator = Substitute.For<IServiceLocator>();
            serviceLocator.GetInstance<ExampleBuilderMock>().Returns(new ExampleBuilderMock());

            var manager = new ServiceLocatingBuildBuddy(serviceLocator);
            manager.BuildUsing<ExampleBuilderMock>();

            serviceLocator.Received().GetInstance<ExampleBuilderMock>();
        }

        [Test]
        public void Created_Builder_Is_Returned()
        {
            var manager = new Core.BuildBuddy();
            var builder = manager.BuildUsing<ExampleBuilderMock>();

            builder.Should().BeOfType<ExampleBuilderMock>();
        }

        [Test]
        public void Created_Builder_References_Same_BuildManager()
        {
            var manager = new Core.BuildBuddy();
            var builder = manager.BuildUsing<ExampleBuilderMock>();

            builder.BuilderManager.Should().BeSameAs(manager);
        }

        [Test]
        public void Customizations_Are_Performed_On_Built_Instance()
        {
            var manager = new Core.BuildBuddy();
            string customString = Generate.RandomString(50);
            ExampleClass result = manager.BuildUsing<ExampleBuilderMock>()
                .Customize(b => b.StringProp = customString)
                .Create();

            result.StringProp.Should().Be(customString, "customize was used to set the property.");
        }

        [Test]
        public void Second_Create_Builds_A_Different_Instance()
        {
            var manager = new Core.BuildBuddy();
            ExampleBuilderMock builderMock = manager.BuildUsing<ExampleBuilderMock>();
            
            ExampleClass result1 = builderMock.Create();
            ExampleClass result2 = builderMock.Create();

            result1.Should().NotBeSameAs(result2);
        }

        [Test]
        public void Multiple_Creates_Pass_Seeds_To_Build_Method()
        {
            var manager = new Core.BuildBuddy();
            ExampleBuilderMock builderMock = manager.BuildUsing<ExampleBuilderMock>();

            builderMock.Create(42);
            builderMock.Create(1);

            builderMock.WasAskedToConstructWithSeeds.Should().ContainInOrder(new[] { 42, 1 });
        }

        [Test]
        public void CreateMany_Builds_Different_Instances()
        {
            var manager = new Core.BuildBuddy();
            ExampleBuilderMock builderMock = manager.BuildUsing<ExampleBuilderMock>();

            ExampleClass[] results = builderMock.CreateMany(3).ToArray();

            results.Should().OnlyHaveUniqueItems();
        }

        [Test]
        public void CreateMany_Pass_Sequental_Seeds_To_Build_Method()
        {
            var manager = new Core.BuildBuddy();
            ExampleBuilderMock builderMock = manager.BuildUsing<ExampleBuilderMock>();

            builderMock.CreateMany(3);

            builderMock.WasAskedToConstructWithSeeds.Should().ContainInOrder(new[] { 0, 1, 2 });
        }
    }
}