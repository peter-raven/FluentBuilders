using Microsoft.Practices.ServiceLocation;

namespace BuildBuddy.Core
{
    public class BuilderManager
    {
        private readonly IServiceLocator _serviceLocator;

        public BuilderManager(IServiceLocator serviceLocator)
        {
            _serviceLocator = serviceLocator;
        }

        public BuilderManager()
        {
            _serviceLocator = new SimpleServiceLocator();
        }

        public T ConstructUsing<T>() where T : BuilderSetupBase
        {
            T factory = _serviceLocator.GetInstance<T>();
            factory.BuilderManager = this;
            return factory;
        }
    }
}
