using System.Linq;
using BuildBuddy.Core;
using FluentAssertions;
using Microsoft.Practices.ServiceLocation;
using NSubstitute;
using NUnit.Framework;

namespace BuildBuddy.Tests
{
    [TestFixture]
    public class When_Creating_A_Builder_Using_BuildBuddy
    {
        [Test]
        public void The_Supplied_Service_Locator_Is_Asked_For_Factory_Instance()
        {
            var serviceLocator = Substitute.For<IServiceLocator>();
            serviceLocator.GetInstance<BuilderMock>().Returns(new BuilderMock());

            var manager = new BuilderManager(serviceLocator);
            manager.ConstructUsing<BuilderMock>();

            serviceLocator.Received().GetInstance<BuilderMock>();
        }

        [Test]
        public void Created_Builder_Is_Returned()
        {
            var manager = new BuilderManager();
            var builder = manager.ConstructUsing<BuilderMock>();

            builder.Should().BeOfType<BuilderMock>();
        }

        [Test]
        public void Created_Builder_References_Same_BuildBuddy()
        {
            var manager = new BuilderManager();
            var builder = manager.ConstructUsing<BuilderMock>();

            builder.BuilderManager.Should().BeSameAs(manager);
        }

        [Test]
        public void Customizations_Are_Performed_On_Built_Instance()
        {
            var manager = new BuilderManager();
            string customString = Generate.RandomString(50);
            ExampleClass result = manager.ConstructUsing<BuilderMock>()
                .Customize(b => b.StringProp = customString)
                .Create();

            result.StringProp.Should().Be(customString, "customize was used to set the property.");
        }

        [Test]
        public void Second_Create_Builds_A_Different_Instance()
        {
            var manager = new BuilderManager();
            BuilderMock builderMock = manager.ConstructUsing<BuilderMock>();
            
            ExampleClass result1 = builderMock.Create();
            ExampleClass result2 = builderMock.Create();

            result1.Should().NotBeSameAs(result2);
        }

        [Test]
        public void Multiple_Creates_Pass_Seeds_To_Build_Method()
        {
            var manager = new BuilderManager();
            BuilderMock builderMock = manager.ConstructUsing<BuilderMock>();

            builderMock.Create(42);
            builderMock.Create(1);

            builderMock.WasAskedToConstructWithSeeds.Should().ContainInOrder(new[] { 42, 1 });
        }

        [Test]
        public void CreateMany_Builds_Different_Instances()
        {
            var manager = new BuilderManager();
            BuilderMock builderMock = manager.ConstructUsing<BuilderMock>();

            ExampleClass[] results = builderMock.CreateMany(3).ToArray();

            results.Should().OnlyHaveUniqueItems();
        }

        [Test]
        public void CreateMany_Pass_Sequental_Seeds_To_Build_Method()
        {
            var manager = new BuilderManager();
            BuilderMock builderMock = manager.ConstructUsing<BuilderMock>();

            builderMock.CreateMany(3);

            builderMock.WasAskedToConstructWithSeeds.Should().ContainInOrder(new[] { 0, 1, 2 });
        }
    }
}