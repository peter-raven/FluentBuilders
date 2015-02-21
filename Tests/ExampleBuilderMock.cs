using System.Collections.Generic;
using BuildBuddy.Core;

namespace BuildBuddy.Tests
{
    public class ExampleBuilderMock : Builder<ExampleClass>
    {
        public bool WasAskedToConstruct { get; set; }
        public List<int> WasAskedToConstructWithSeeds { get; private set; }
        public string Something { get; set; }

        public ExampleBuilderMock() : this(new SimpleBuilderManager())
        {
        }

        public ExampleBuilderMock(IBuilderManager mgr) : base(mgr)
        {
            WasAskedToConstructWithSeeds = new List<int>();
        }

        public ExampleBuilderMock WithSomething(string something)
        {
            Something = something;
            return this;
        }

        protected override ExampleClass Build(int seed)
        {
            WasAskedToConstruct = true;
            WasAskedToConstructWithSeeds.Add(seed);
            return new ExampleClass();
        }
    }
}