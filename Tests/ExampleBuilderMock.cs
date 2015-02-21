using System;
using System.Collections.Generic;
using BuildBuddy.Core;

namespace BuildBuddy.Tests
{
    public class ExampleBuilderMock : Builder<ExampleClass>
    {
        private CollectionBuilder<ExampleChildClass, ExampleChildBuilderMock> _childBuilders;
        public bool WasAskedToConstruct { get; set; }
        public List<int> WasAskedToConstructWithSeeds { get; private set; }
        public string _stringProp { get; set; }
        
        public ExampleBuilderMock() : this(new Core.BuildBuddy())
        {
        }

        public ExampleBuilderMock(IBuilderManager mgr) : base(mgr)
        {
            WasAskedToConstructWithSeeds = new List<int>();
            _childBuilders = new CollectionBuilder<ExampleChildClass, ExampleChildBuilderMock>(this);
        }

        public ExampleBuilderMock WithStringProp(string something)
        {
            _stringProp = something;
            return this;
        }

        public ExampleBuilderMock WithChildren(Action<CollectionBuilder<ExampleChildClass, ExampleChildBuilderMock>> opts)
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
                StringProp = _stringProp
            };
            res.Children.AddRange(_childBuilders.CreateAll());

            return res;
        }
    }
}