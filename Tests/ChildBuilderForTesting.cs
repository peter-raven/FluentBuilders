using System.Collections.Generic;
using BuildBuddy.Core;

namespace BuildBuddy.Tests
{
    public class ChildBuilderForTesting : Builder<ExampleChildClass>
    {
        public bool WasAskedToConstruct { get; set; }
        public List<int> WasAskedToConstructWithSeeds { get; private set; }
        public string Something { get; set; }

        public ChildBuilderForTesting()
        {
            WasAskedToConstructWithSeeds = new List<int>();
        }

        public ChildBuilderForTesting WithSomething(string something)
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