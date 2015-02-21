using System;
using System.Reflection;

namespace BuildBuddy.Core
{
    public class BuildBuddy : IBuilderManager
    {
        public T BuildUsing<T>()
        {
            Type type = typeof(T);
            ConstructorInfo ctor = type.GetConstructor(new[] { typeof(IBuilderManager) });
            if (ctor == null)
                return (T)Activator.CreateInstance(typeof(T));
            
            return (T) ctor.Invoke(new object[] { this });
        }
    }
}