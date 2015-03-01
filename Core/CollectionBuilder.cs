using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace BuildBuddy.Core
{
    /// <summary>
    /// Helper for building child collections to your entity. Use this to expose a fluent syntax for building up child collections.
    /// </summary>
    /// <typeparam name="T">Type of child entity in the child collection</typeparam>
    /// <typeparam name="TBuilder">Type of builder to build the child entities.</typeparam>
    public class CollectionBuilder<T, TBuilder>
        where T : class
        where TBuilder : Builder<T>
    {
        private readonly Func<TBuilder> _factoryResolver;
        private readonly List<TBuilder> _builders;

        public CollectionBuilder(IBuilder parentFactory)
        {
            _factoryResolver = parentFactory.BuildUsing<TBuilder>;
            _builders = new List<TBuilder>();
        }

        public List<TBuilder> Builders
        {
            get { return _builders; }
        }

        /// <summary>
        /// Add one entity that will be created by a builder to this child collection.
        /// </summary>
        /// <returns></returns>
        public TBuilder AddOne()
        {
            return AddMany(1);
        }

        /// <summary>
        /// Add specified instance to this child collection.
        /// </summary>
        /// <param name="toAdd">Instance to add.</param>
        public void AddOne(T toAdd)
        {
            AddMany(new[] { toAdd });
        }

        /// <summary>
        /// Add specified instances to this child collection.
        /// </summary>
        /// <param name="addItems">Instances to add.</param>
        public void AddMany(IEnumerable<T> addItems)
        {
            foreach (var a in addItems)
                _builders.Add((TBuilder) _factoryResolver().WithInstance(a));
        }

        /// <summary>
        /// Adds multiple entities that will each be created by a builder to this child collection
        /// </summary>
        /// <param name="count">Number of entities to create</param>
        /// <param name="opts">Alterations that should be done to the builder of each item.</param>
        /// <returns></returns>
        public TBuilder AddMany(int count, Action<TBuilder> opts = null)
        {
            var fac = _factoryResolver();
            for (int i = 0; i < count; i++)
            {
                if (opts != null)
                    opts(fac);
                _builders.Add(fac);
            }
            return fac;
        }

        /// <summary>
        /// Processes this collection builder, creating all instances.
        /// </summary>
        /// <param name="setupAction">Optional action to apply to each builder.</param>
        /// <param name="customization">Optional action to apply to each created entity.</param>
        /// <returns>All instances created by this collection builder.</returns>
        public IEnumerable<T> CreateAll(Action<TBuilder> setupAction = null, Action<T> customization = null)
        {
            for (int i = 0; i < _builders.Count; i++)
            {
                TBuilder builder = _builders[i];
                if (setupAction != null)
                    setupAction(builder);
                T result = builder.Create(i);
                if (customization != null)
                    customization(result);
                yield return result;
            }
        }
    }
}