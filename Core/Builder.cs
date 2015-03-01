using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace BuildBuddy.Core
{
    public abstract class Builder<TSubject> : IBuilder<TSubject>
        where TSubject : class
    {
        protected Dictionary<string, IBuilder> OptIns { get; set; } 
        protected readonly List<Action<TSubject>> Customizations;
        protected TSubject Instance { get; set; }
        public BuilderFactoryConvention BuilderFactoryConvention { get; set; }
        
        protected Builder()
        {
            OptIns = new Dictionary<string, IBuilder>();
            Customizations = new List<Action<TSubject>>();
            BuilderFactoryConvention = new BuilderFactoryConvention();
            Instance = null;
        }

        public Builder<TSubject> WithInstance(TSubject instance)
        {
            Instance = instance;
            return this;
        }

        public void OptInWith<T>(Expression<Func<TSubject, object>> prop, T instance)
        {
            OptInWithBuilder(prop, new ObjectContainer<T>(instance));
        }

        public void OptInWith<TNestedBuilder>(Expression<Func<TSubject, object>> prop, Action<TNestedBuilder> opts = null) where TNestedBuilder : IBuilder
        {
            TNestedBuilder builder = BuildUsing<TNestedBuilder>();
            if (opts != null)
                opts(builder);
            OptInWithBuilder<TNestedBuilder>(prop, builder);
        }

        private void OptInWithBuilder<TNestedBuilder>(Expression<Func<TSubject, object>> prop, TNestedBuilder builder) where TNestedBuilder : IBuilder
        {
            MemberExpression member = (MemberExpression)prop.Body;
            string key = member.Member.Name;
            if (OptIns.ContainsKey(key))
                throw new InvalidOperationException(String.Format(
                    "The builder was told to set property {0} of {1} more than once. " +
                    "Now it does not know what to do." +
                    "Check if you somehow asked the builder to set this property multiple times.", key, typeof(TSubject).Name));
            OptIns.Add(key, builder);
        }

        protected T OptInFor<T>(Expression<Func<TSubject, T>> prop, Func<T> valueIfNoOptIn)
        {
            MemberExpression member = (MemberExpression)prop.Body;
            string key = member.Member.Name;
            if (!OptIns.ContainsKey(key))
                return valueIfNoOptIn();
            return (T) OptIns[key].Create();
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

        public T BuildUsing<T>() where T : IBuilder
        {
            return BuilderFactoryConvention.Create<T>();
        }
    }
}