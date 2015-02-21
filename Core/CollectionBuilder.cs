using System;
using System.Collections.Generic;

namespace BuildBuddy.Core
{
    // xyz.WithOrderLines(x => x.AddOne())

    public class CollectionBuilder<TParentBuilder, T, TBuilder>
        where T : class
        where TParentBuilder : IBuilder
        where TBuilder : Builder<T>
    {
        private readonly TParentBuilder _parentFactory;
        private readonly Func<TBuilder> _factoryResolver;
        private readonly BuilderList<T, TBuilder> _builders;

        public CollectionBuilder(TParentBuilder parentFactory)
        {
            _parentFactory = parentFactory;
            _factoryResolver = _parentFactory.BuildUsing<TBuilder>;
            _builders = new BuilderList<T, TBuilder>();
        }

        public BuilderList<T, TBuilder> Builders
        {
            get { return _builders; }
        }

        public TParentBuilder AddOne()
        {
            return AddMany(1);
        }

        public TParentBuilder AddOne(T toAdd)
        {
            return AddMany(new[] { toAdd });
        }

        public TParentBuilder AddOne(Action<TBuilder> opts)
        {
            return AddMany(1, opts);
        }

        public TParentBuilder AddMany(IEnumerable<T> addItems)
        {
            foreach (var a in addItems)
                _builders.Add(new BuilderTuple<T, TBuilder>(a));
            return _parentFactory;
        }

        public TParentBuilder AddMany(int count, Action<TBuilder> opts = null)
        {
            for (int i = 0; i < count; i++)
            {
                var fac = _factoryResolver();
                if (opts != null)
                    opts(fac);
                _builders.Add(new BuilderTuple<T, TBuilder>(fac));
            }
            return _parentFactory;
        }
    }
}