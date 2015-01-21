using System;

namespace BuildBuddy.Core
{
    public class BuilderTuple<T, TFactory>
        where T : class
        where TFactory : BuilderSetup<TFactory, T>
    {
        public T Instance { get; private set; }
        public TFactory Factory { get; private set; }

        public BuilderTuple(T instance)
        {
            Instance = instance;
            Factory = null;
        }

        public BuilderTuple(TFactory factory)
        {
            Instance = null;
            Factory = factory;
        }

        public T Resolve(Action<TFactory> factoryOpts = null)
        {
            if (Instance != null)
                return Instance;
            if (factoryOpts != null)
                factoryOpts(Factory);
            return Factory.Create();
        }
    }
}