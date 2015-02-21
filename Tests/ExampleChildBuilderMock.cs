using System.Collections.Generic;
using BuildBuddy.Core;

namespace BuildBuddy.Tests
{
    public class ExampleChildBuilderMock : Builder<ExampleChildClass>
    {
        public bool WasAskedToConstruct { get; set; }
        public List<int> WasAskedToConstructWithSeeds { get; private set; }
        public string Something { get; set; }

        public ExampleChildBuilderMock() : base(new Core.BuildBuddy())
        {
            WasAskedToConstructWithSeeds = new List<int>();
        }

        public ExampleChildBuilderMock WithSomething(string something)
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