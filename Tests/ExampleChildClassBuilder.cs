using System.Collections.Generic;
using FluentBuilders.Core;

namespace FluentBuilders.Tests
{
    public class ExampleChildClassBuilder : Builder<ExampleChildClass>
    {
        public bool WasAskedToConstruct { get; set; }
        public List<int> WasAskedToConstructWithSeeds { get; private set; }
        public string Something { get; set; }

        public ExampleChildClassBuilder()
        {
            WasAskedToConstructWithSeeds = new List<int>();
        }

        public ExampleChildClassBuilder WithSomething(string something)
        {
            Something = something;
            return this;
        }

        protected override ExampleChildClass Build(int seed)
        {
            WasAskedToConstruct = true;
            WasAskedToConstructWithSeeds.Add(seed);
            return new ExampleChildClass();
        }
    }
}