using System.Collections.Generic;
using BuildBuddy.Core;

namespace BuildBuddy.Tests
{
    public class BuilderMock : BuilderSetup<BuilderMock, ExampleClass>
    {
        public bool WasAskedToConstruct { get; set; }
        public List<int> WasAskedToConstructWithSeeds { get; private set; }

        public BuilderMock()
        {
            WasAskedToConstructWithSeeds = new List<int>();
        }

        public BuilderMock WithSomething(string something)
        {
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