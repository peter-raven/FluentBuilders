using System;
using FluentBuilders.Core;

namespace FluentBuilders.Tests
{
    public class ExampleClassBuilder : Builder<ExampleClass>
    {
        private CollectionBuilder<ExampleChildClass, ChildBuilderForTesting> _childBuilders;

        public ExampleClassBuilder()
        {
            _childBuilders = new CollectionBuilder<ExampleChildClass, ChildBuilderForTesting>(this);
        }

        public ExampleClassBuilder WithStringProp(string something)
        {
            OptInWith(x => x.StringProp, something);
            return this;
        }

        public ExampleClassBuilder WithDateTimeProp(DateTime someDate)
        {
            OptInWith(x => x.DateProp, someDate);
            return this;
        }

        public ExampleClassBuilder WithReferenceProp(ExampleClass obj)
        {
            OptInWith(x => x.ReferenceProp, obj);
            return this;
        }

        public ExampleClassBuilder WithReferenceProp(Action<ExampleClassBuilder> opts = null)
        {
            OptInWith<ExampleClassBuilder>(x => x.ReferenceProp, opts);
            return this;
        }

        public ExampleClassBuilder WithChildren(Action<CollectionBuilder<ExampleChildClass, ChildBuilderForTesting>> opts)
        {
            opts(_childBuilders);
            return this;
        }

        protected override ExampleClass Build(int seed)
        {
            var res = new ExampleClass
            {
                StringProp = OptInFor(x => x.StringProp, () => "default"),
                DateProp = OptInFor(x => x.DateProp, () => DateTime.MinValue)
            };
            res.ReferenceProp = OptInFor(x => x.ReferenceProp, () => res);
            res.Children.AddRange(_childBuilders.CreateAll());

            return res;
        }
    }
}