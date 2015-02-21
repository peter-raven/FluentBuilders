using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace BuildBuddy.Core
{
    public abstract class Builder<TSubject> : IBuilder
        where TSubject : class
    {
        protected readonly List<Action<TSubject>> Customizations;
        internal IBuilderManager BuilderManager { get; set; }

        protected Builder(IBuilderManager manager)
        {
            BuilderManager = manager;
            Customizations = new List<Action<TSubject>>();
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
            TSubject subject = Build(seed);
            foreach (var cust in Customizations)
                cust(subject);
            
            return subject;
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
            return BuilderManager.BuildUsing<T>();
        }
    }
}