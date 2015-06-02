using System;
using FluentAssertions;
using FluentBuilders.Core;
using NUnit.Framework;

namespace FluentBuilders.Tests
{
    [TestFixture]
    public class When_Using_SetProperty
    {
        [Test]
        public void A_String_Prop_Can_Be_Set()
        {
            var builder = new BuilderForTesting<ExampleClass>();
            string str = "abc";

            builder.SetProperty(x => x.StringProp, str);
            string res = builder.GetProperty(x => x.StringProp, String.Empty);

            res.Should().Be(str, "String prop should be set on the created class");
        }

        [Test]
        public void A_DateTime_Prop_Can_Be_Set()
        {
            var builder = new BuilderForTesting<ExampleClass>();
            DateTime now = DateTime.Now;

            builder.SetProperty(x => x.DateProp, now);
            DateTime res = builder.GetProperty(x => x.DateProp, DateTime.MinValue);

            res.Should().Be(now, "DateTime prop should be set on the created class");
        }

        [Test]
        public void Can_Use_Nested_Builder()
        {
            var builder = new BuilderForTesting<ExampleClass>();
            
            builder.SetProperty<ExampleReferencedClassBuilder>(x => x.ReferenceProp);
            ExampleReferencedClass res = builder.GetProperty(x => x.ReferenceProp, () => null);

            res.Should().NotBeNull("Nested builder should create an instance.");
        }

        [Test]
        public void Can_Use_Nested_Builder_With_SetProperty()
        {
            var builder = new BuilderForTesting<ExampleClass>();

            builder.SetProperty<ExampleReferencedClassBuilder>(x => x.ReferenceProp, opts: o => o.WithStringProp("abc"));
            ExampleReferencedClass res = builder.GetProperty(x => x.ReferenceProp, () => null);

            res.Should().NotBeNull("Nested builder should create an instance.");
            res.StringProp.Should().Be("abc", "Nested builder should create an instance with applied opt-ins for the builder.");
        }

        [Test]
        public void A_Property_Can_Be_Set_Using_Arbitrary_Key()
        {
            var builder = new BuilderForTesting<ExampleClass>();

            builder.SetProperty("mykey", "my opt in");
            string res = builder.GetProperty("mykey", "default");

            res.Should().Be("my opt in", "");
        }

        [Test]
        public void GetProperty_Default_Func_Is_Not_Ivoked_If_SetProperty_Exists()
        {
            var builder = new BuilderForTesting<ExampleClass>();
            string str = Generate.RandomString(10);
            bool wasInvoked = false;

            builder.SetProperty(x => x.StringProp, str);
            builder.GetProperty(x => x.StringProp, () => { wasInvoked = true; return String.Empty; });

            wasInvoked.Should().BeFalse("Opt in was present, so default func should not be invoked");
        }

        [Test]
        public void GetProperty_Default_Func_Is_Ivoked_If_SetProperty_Does_Not_Exist()
        {
            var builder = new BuilderForTesting<ExampleClass>();
            bool wasInvoked = false;

            builder.GetProperty(x => x.StringProp, () => { wasInvoked = true; return String.Empty; });

            wasInvoked.Should().BeTrue("Opt in was not present, so default func should be invoked");
        }
    }
}