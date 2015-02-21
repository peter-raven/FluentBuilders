using System;

namespace BuildBuddy.Core
{
    public class BuilderTuple<T, TBuilder>
        where T : class
        where TBuilder : Builder<T>
    {
        public T Instance { get; private set; }
        public TBuilder Builder { get; private set; }

        public BuilderTuple(T instance)
        {
            Instance = instance;
            Builder = null;
        }

        public BuilderTuple(TBuilder builder)
        {
            Instance = null;
            Builder = builder;
        }

        public T Resolve(Action<TBuilder> factoryOpts = null)
        {
            if (Instance != null)
                return Instance;
            if (factoryOpts != null)
                factoryOpts(Builder);
            return Builder.Create();
        }
    }
}