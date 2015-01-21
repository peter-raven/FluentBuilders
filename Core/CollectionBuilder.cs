using System;
using System.Collections.Generic;

namespace BuildBuddy.Core
{
    public class CollectionBuilder<TParentFactory, T, TFactory>
        where TParentFactory : BuilderSetupBase
        where T : class
        where TFactory : BuilderSetup<TFactory, T>
    {
        private readonly TParentFactory _parentFactory;
        private readonly Func<TFactory> _factoryResolver;
        private readonly BuilderList<T, TFactory> _builders;

        public CollectionBuilder(TParentFactory parentFactory, Func<TFactory> factoryResolver = null)
        {
            _parentFactory = parentFactory;
            _factoryResolver = (() => _parentFactory.BuilderManager.ConstructUsing<TFactory>());
            _builders = new BuilderList<T, TFactory>();
        }

        public BuilderList<T, TFactory> Builders
        {
            get { return _builders; }
        }

        public TParentFactory AddOne()
        {
            return AddMany(1);
        }

        public TParentFactory AddOne(T toAdd)
        {
            return AddMany(new[] { toAdd });
        }

        public TParentFactory AddOne(Action<TFactory> opts)
        {
            return AddMany(1, opts);
        }

        public TParentFactory AddMany(IEnumerable<T> addItems)
        {
            foreach (var a in addItems)
                _builders.Add(new BuilderTuple<T, TFactory>(a));
            return _parentFactory;
        }

        public TParentFactory AddMany(int count, Action<TFactory> opts = null)
        {
            for (int i = 0; i < count; i++)
            {
                var fac = _factoryResolver();
                if (opts != null)
                    opts(fac);
                _builders.Add(new BuilderTuple<T, TFactory>(fac));
            }
            return _parentFactory;
        }
    }
}