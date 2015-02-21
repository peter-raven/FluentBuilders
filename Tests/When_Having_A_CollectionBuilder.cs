using BuildBuddy.Core;
using FluentAssertions;
using NUnit.Framework;

namespace BuildBuddy.Tests
{
    [TestFixture]
    public class When_Having_A_CollectionBuilder
    {
        [Test]
        public void AddOne_Will_Add_Item_Builder_To_Builders()
        {
            var parentBuilder = new ExampleBuilderMock();
            var t = new CollectionBuilder<ExampleChildClass, ExampleChildBuilderMock>(parentBuilder);

            t.AddOne();

            t.Builders.Count.Should().Be(1, "only one builder should be added");
            t.Builders.Should().ContainItemsAssignableTo<ExampleChildBuilderMock>();
        }

        [Test]
        public void AddOne_Will_Return_The_Builder_It_Just_Added_To_Builders()
        {
            var parentBuilder = new ExampleBuilderMock();
            var t = new CollectionBuilder<ExampleChildClass, ExampleChildBuilderMock>(parentBuilder);

            var res = t.AddOne();

            t.Builders.Should().OnlyContain(x => x == res);
        }

        [Test]
        public void AddOne_With_Instance_Will_Add_Item_Builder_With_That_Instance_To_Builders()
        {
            var parentBuilder = new ExampleBuilderMock();
            var t = new CollectionBuilder<ExampleChildClass, ExampleChildBuilderMock>(parentBuilder);
            var c = new ExampleChildClass();

            t.AddOne(c);

            t.Builders.Count.Should().Be(1);
            t.CreateAll().Should().OnlyContain(x => x == c);
        }

        [Test]
        public void AddMany_With_Instances_Will_Add_Item_Builders_With_These_Instances_To_Builders()
        {
            var parentBuilder = new ExampleBuilderMock();
            var t = new CollectionBuilder<ExampleChildClass, ExampleChildBuilderMock>(parentBuilder);
            var c = new[] { new ExampleChildClass(), new ExampleChildClass(), new ExampleChildClass() };

            t.AddMany(c);

            t.Builders.Count.Should().Be(3);
            t.CreateAll().Should().Contain(c);
        }

        [Test]
        public void AddMany_Will_Reuse_Same_Item_Builder()
        {
            var parentBuilder = new ExampleBuilderMock();
            var t = new CollectionBuilder<ExampleChildClass, ExampleChildBuilderMock>(parentBuilder);
            
            t.AddMany(3);

            t.Builders.Count.Should().Be(3);
            t.Builders.Should().OnlyContain(x => x == t.Builders[0]);
        }

        [Test]
        public void AddMany_With_Builder_Options_Will_Apply_These_Options_To_Each_Builder()
        {
            var parentBuilder = new ExampleBuilderMock();

            var test = parentBuilder
                .WithChildren(c => c
                    .AddOne())
                .Create();

            var t = new CollectionBuilder<ExampleChildClass, ExampleChildBuilderMock>(parentBuilder);

            t.AddMany(3, opts => opts.WithSomething("ploeh"));

            t.Builders.Should().OnlyContain(x => x.Something == "ploeh");
        }
    }
}