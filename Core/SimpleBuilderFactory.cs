using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace BuildBuddy.Core
{
    public class SimpleBuilderFactory : IBuilderFactory
    {
        public T Create<T>()
        {
            Type type = typeof(T);
            ConstructorInfo[] ctors = type.GetConstructors();
            if (ctors.Any(x => x.GetParameters().Length == 0))
                return (T)Activator.CreateInstance(typeof(T));

            throw new InvalidOperationException(String.Format(
                @"Cannot create a new builder of type {0}, because it does not have a parameterless constructor.
You might need to create your own IBuilderFactory that can instantiate the builder with parameters in the constructor.
Put it to use by using BuilderFactoryConvention.UseFactory of the parent builder.", type.Name));
        }
    }
}
