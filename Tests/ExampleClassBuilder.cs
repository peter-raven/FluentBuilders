using System;
using FluentBuilders.Core;

namespace FluentBuilders.Tests
{
    public class ExampleClassBuilder : Builder<ExampleClass>
    {
        public ExampleClassBuilder WithStringProp(string something)
        {
            SetProperty(x => x.StringProp, something);
            return this;
        }

        public ExampleClassBuilder WithDateTimeProp(DateTime someDate)
        {
            SetProperty(x => x.DateProp, someDate);
            return this;
        }

        public ExampleClassBuilder WithReferenceProp(ExampleClass obj)
        {
            SetProperty(x => x.ReferenceProp, obj);
            return this;
        }

        public ExampleClassBuilder WithReferenceProp(Action<ExampleReferencedClassBuilder> opts = null)
        {
            SetProperty<ExampleReferencedClassBuilder>(x => x.ReferenceProp, opts);
            return this;
        }

        public ExampleClassBuilder WithChildren(Action<CollectionBuilder<ExampleChildClass, ExampleChildClassBuilder>> opts)
        {
            SetCollection(x => x.Children, opts);
            return this;
        }

        protected override ExampleClass Build(int seed)
        {
            var res = new ExampleClass
            {
                StringProp = GetProperty(x => x.StringProp, () => "default"),
                DateProp = GetProperty(x => x.DateProp, () => DateTime.MinValue)
            };

            res.ReferenceProp = GetPropertyBuilder(x => x.ReferenceProp, orUse: BuildUsing<ExampleReferencedClassBuilder>)
                .WithStringProp("I am default")
                .Create();

            res.Children.AddRange(GetCollection<ExampleChildClass, ExampleChildClassBuilder>(x => x.Children).CreateAll());
            
            return res;
        }
    }
}