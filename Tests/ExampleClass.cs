using System;
using System.Collections.Generic;

namespace FluentBuilders.Tests
{
    public class ExampleClass
    {
        public string StringProp { get; set; }
        public DateTime DateProp { get; set; }
        public ExampleReferencedClass ReferenceProp { get; set; }
        public List<ExampleChildClass> Children { get; private set; }

        public ExampleClass()
        {
            Children = new List<ExampleChildClass>();
        }
    }
}