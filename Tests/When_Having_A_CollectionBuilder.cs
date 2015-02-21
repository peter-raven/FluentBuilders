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
            var t = new CollectionBuilder<ExampleBuilderMock, ExampleChildClass, ExampleChildBuilderMock>(parentBuilder);

            t.AddOne();

            t.Builders.Should().OnlyContain(x => x.Builder != null);
        }
    }
}