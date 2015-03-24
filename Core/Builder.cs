using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace FluentBuilders.Core
{
    /// <summary>
    /// Abstract base class for fluent builders.
    /// </summary>
    /// <typeparam name="TSubject">Type of object this builder will build.</typeparam>
    public abstract class Builder<TSubject> : IBuilder<TSubject>
        where TSubject : class
    {
        protected Dictionary<string, IBuilder> OptIns { get; set; } 
        protected readonly List<Action<TSubject>> Customizations;
        public TSubject Instance { get; set; }
        public BuilderFactoryConvention BuilderFactoryConvention { get; set; }
        
        protected Builder()
        {
            OptIns = new Dictionary<string, IBuilder>();
            Customizations = new List<Action<TSubject>>();
            BuilderFactoryConvention = new BuilderFactoryConvention();
            Instance = null;
        }

        /// <summary>
        /// Instructs the builder to return the specified instance each time Create is invoked.
        /// </summary>
        /// <param name="instance">Fixed instance to return</param>
        /// <returns></returns>
        public Builder<TSubject> WithInstance(TSubject instance)
        {
            Instance = instance;
            return this;
        }

        /// <summary>
        /// Instructs the builder to set the specified property of the created instance to a specific value.
        /// </summary>
        /// <typeparam name="T">The property's type</typeparam>
        /// <param name="prop">Lambda expression pointing out the property</param>
        /// <param name="instance">The instance/value for the property</param>
        protected void OptInWith<T>(Expression<Func<TSubject, object>> prop, T instance)
        {
            OptInWithBuilder(prop, new ObjectContainer<T>(instance));
        }

        /// <summary>
        /// Instructs the builder to keep a specific instance/value for later use, stored using a key.
        /// </summary>
        /// <typeparam name="T">Type of instance to keep</typeparam>
        /// <param name="key">Key to use for storing</param>
        /// <param name="instance">Instance to keep</param>
        protected void OptInWith<T>(string key, T instance)
        {
            OptInWithBuilder(key, new ObjectContainer<T>(instance));
        }

        /// <summary>
        /// Instructs the builder that the specified property should be set using a nested builder, with optional settings on the nested builder.
        /// </summary>
        /// <typeparam name="TNestedBuilder">Type of nested builder</typeparam>
        /// <param name="prop">Lambda expression pointing out the property</param>
        /// <param name="opts">Optional actions to perform on the nested builder.</param>
        protected void OptInWith<TNestedBuilder>(Expression<Func<TSubject, object>> prop, Action<TNestedBuilder> opts = null) where TNestedBuilder : IBuilder
        {
            TNestedBuilder builder = BuildUsing<TNestedBuilder>();
            if (opts != null)
                opts(builder);
            OptInWithBuilder<TNestedBuilder>(prop, builder);
        }

        /// <summary>
        /// Instructs the builder that the specified key store the specified nested builder, with optional settings on the nested builder.
        /// </summary>
        /// <typeparam name="TNestedBuilder"></typeparam>
        /// <param name="key"></param>
        /// <param name="opts"></param>
        protected void OptInWith<TNestedBuilder>(string key, Action<TNestedBuilder> opts = null) where TNestedBuilder : IBuilder
        {
            TNestedBuilder builder = BuildUsing<TNestedBuilder>();
            if (opts != null)
                opts(builder);
            OptInWithBuilder<TNestedBuilder>(key, builder);
        }

        private void OptInWithBuilder<TNestedBuilder>(Expression<Func<TSubject, object>> prop, TNestedBuilder builder) where TNestedBuilder : IBuilder
        {
            MemberExpression member = (MemberExpression)prop.Body;
            string key = member.Member.Name;
            OptInWithBuilder(key, builder);
        }

        private void OptInWithBuilder<TNestedBuilder>(string key, TNestedBuilder builder) where TNestedBuilder : IBuilder
        {
            if (OptIns.ContainsKey(key))
                throw new InvalidOperationException(String.Format(
                    "The builder was told to set property {0} of {1} more than once. " +
                    "Now it does not know what to do. " +
                    "Check if you somehow asked the builder to set this property multiple times.", key, typeof(TSubject).Name));
            OptIns.Add(key, builder);
        }

        /// <summary>
        /// Gets the value/instance set by opt-in for the specified property.
        /// </summary>
        /// <typeparam name="T">Type of the property</typeparam>
        /// <param name="prop">Lambda expression pointing to the property</param>
        /// <param name="valueIfNoOptIn">Value or instance to return if no opt-in was registered for the property.</param>
        /// <returns></returns>
        protected T OptInFor<T>(Expression<Func<TSubject, T>> prop, Func<T> valueIfNoOptIn)
        {
            MemberExpression member = (MemberExpression)prop.Body;
            string key = member.Member.Name;
            if (!OptIns.ContainsKey(key))
                return valueIfNoOptIn();
            return (T) OptIns[key].Create();
        }

        protected T OptInFor<T>(Expression<Func<TSubject, T>> prop, T valueIfNoOptIn)
        {
            MemberExpression member = (MemberExpression)prop.Body;
            string key = member.Member.Name;
            if (!OptIns.ContainsKey(key))
                return valueIfNoOptIn;
            return (T)OptIns[key].Create();
        }

        /// <summary>
        /// Adds a customization to the builder that will be applied to the instance being built.
        /// </summary>
        /// <param name="action">Customization to apply to the built instance. Will be applied as the last step in building the instance.</param>
        /// <returns></returns>
        public Builder<TSubject> Customize(Action<TSubject> action)
        {
            Customizations.Add(action);
            return this;
        }

        /// <summary>
        /// Creates an instance using this builder.
        /// </summary>
        /// <param name="seed">Optional seed to provide to the construction process</param>
        /// <returns></returns>
        public virtual TSubject Create(int seed = 0)
        {
            TSubject subject = Instance ?? Build(seed);
            foreach (var cust in Customizations)
                cust(subject);
            
            return subject;
        }

        object IBuilder.Create(int seed)
        {
            return Create(seed);
        }

        public static implicit operator TSubject(Builder<TSubject> builder)
        {
            return builder.Create();
        }

        /// <summary>
        /// Creates multiple instances using this builder.
        /// </summary>
        /// <param name="i">Number of instances to create</param>
        /// <returns>The given number of instances.</returns>
        public virtual IEnumerable<TSubject> CreateMany(int i)
        {
            var objs = new List<TSubject>();
            for (int x = 0; x < i; x++)
            {
                objs.Add(Create(x));
            }

            return objs;
        }

        protected abstract TSubject Build(int seed);

        /// <summary>
        /// Creates a nested builder using the current builder factory convention.
        /// </summary>
        /// <typeparam name="T">Type of nested build to create.</typeparam>
        /// <returns></returns>
        public T BuildUsing<T>() where T : IBuilder
        {
            return BuilderFactoryConvention.Create<T>();
        }
    }
}