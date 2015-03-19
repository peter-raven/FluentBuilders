using System;
using System.Collections.Generic;
using FluentBuilders.Core;

namespace FluentBuilders.Tests
{
    public class BuilderForTesting : Builder<ExampleClass>
    {
        private CollectionBuilder<ExampleChildClass, ChildBuilderForTesting> _childBuilders;
        public bool WasAskedToConstruct { get; set; }
        public List<int> WasAskedToConstructWithSeeds { get; private set; }

        public BuilderForTesting()
        {
            WasAskedToConstructWithSeeds = new List<int>();
            _childBuilders = new CollectionBuilder<ExampleChildClass, ChildBuilderForTesting>(this);
        }
        
        public BuilderForTesting WithStringProp(string something)
        {
            OptInWith(x => x.StringProp, something);
            return this;
        }

        public BuilderForTesting WithReferenceProp(ExampleClass obj)
        {
            OptInWith(x => x.ReferenceProp, obj);
            return this;
        }

        public BuilderForTesting WithReferenceProp(Action<BuilderForTesting> opts = null)
        {
            OptInWith<BuilderForTesting>(x => x.ReferenceProp, opts);
            return this;
        }

        public BuilderForTesting WithChildren(Action<CollectionBuilder<ExampleChildClass, ChildBuilderForTesting>> opts)
        {
            opts(_childBuilders);
            return this;
        }

        protected override ExampleClass Build(int seed)
        {
            WasAskedToConstruct = true;
            WasAskedToConstructWithSeeds.Add(seed);
            var res = new ExampleClass
            {
                StringProp = OptInFor(x => x.StringProp, () => "default")
            };
            res.ReferenceProp = OptInFor(x => x.ReferenceProp, () => res);
            res.Children.AddRange(_childBuilders.CreateAll());

            return res;
        }
    }
}