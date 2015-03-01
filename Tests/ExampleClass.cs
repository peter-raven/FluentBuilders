using System.Collections.Generic;

namespace BuildBuddy.Tests
{
    public class ExampleClass
    {
        public string StringProp { get; set; }
        public ExampleClass ReferenceProp { get; set; }
        public List<ExampleChildClass> Children { get; private set; }

        public ExampleClass()
        {
            Children = new List<ExampleChildClass>();
        }
    }
}