using System;
using FluentBuilders.Core;

namespace FluentBuilders.Tests
{
    public class ExampleReferencedClassBuilder : Builder<ExampleReferencedClass>
    {
        public ExampleReferencedClassBuilder WithStringProp(string something)
        {
            SetProperty(x => x.StringProp, something);
            return this;
        }

        public ExampleReferencedClassBuilder WithDateTimeProp(DateTime someDate)
        {
            SetProperty(x => x.DateProp, someDate);
            return this;
        }

        protected override ExampleReferencedClass Build(int seed)
        {
            var res = new ExampleReferencedClass
            {
                StringProp = GetProperty(x => x.StringProp, () => "default"),
                DateProp = GetProperty(x => x.DateProp, () => DateTime.MinValue)
            };

            return res;
        }
    }
}