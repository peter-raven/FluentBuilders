using System;
using System.Reflection;
using Microsoft.Practices.ServiceLocation;

namespace BuildBuddy.Core
{
    public class ServiceLocatingBuilderManager : IBuilderManager
    {
        private readonly IServiceLocator _serviceLocator;

        public ServiceLocatingBuilderManager(IServiceLocator serviceLocator)
        {
            _serviceLocator = serviceLocator;
        }

        public T BuildUsing<T>()
        {
            T factory = _serviceLocator.GetInstance<T>();
            return factory;
        }
    }

    public class SimpleBuilderManager : IBuilderManager
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

    public interface IBuilderManager
    {
        T BuildUsing<T>();
    }
}
