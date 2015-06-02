using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;

namespace FluentBuilders.Core
{
    /// <summary>
    /// Abstract base class for fluent builders.
    /// </summary>
    /// <typeparam name="TSubject">Type of object this builder will build.</typeparam>
    public abstract class Builder<TSubject> : IBuilder<TSubject> where TSubject : class
    {
        public List<Action> Setups { get; private set; }
        protected Dictionary<string, IBuilder> PropertyBuilders { get; set; }
        protected readonly List<Action<TSubject>> Customizations;
        public TSubject Instance { get; set; }
        public BuilderFactoryConvention BuilderFactoryConvention { get; set; }
        
        protected Builder()
        {
            Setups = new List<Action>();
            PropertyBuilders = new Dictionary<string, IBuilder>();
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
        protected void SetProperty<T>(Expression<Func<TSubject, object>> prop, T instance)
        {
            SetPropertyBuilder(prop, new ObjectContainer<T>(instance));
        }

        /// <summary>
        /// Instructs the builder to keep a specific instance/value for later use, stored using a key.
        /// </summary>
        /// <typeparam name="T">Type of instance to keep</typeparam>
        /// <param name="key">Key to use for storing</param>
        /// <param name="instance">Instance to keep</param>
        protected void SetProperty<T>(string key, T instance)
        {
            SetPropertyBuilder(key, new ObjectContainer<T>(instance));
        }

        /// <summary>
        /// Instructs the builder that the specified property should be set using a nested builder, with optional settings on the nested builder.
        /// </summary>
        /// <typeparam name="TNestedBuilder">Type of nested builder</typeparam>
        /// <param name="prop">Lambda expression pointing out the property</param>
        /// <param name="opts">Optional actions to perform on the nested builder.</param>
        protected void SetProperty<TNestedBuilder>(Expression<Func<TSubject, object>> prop, Action<TNestedBuilder> opts = null) where TNestedBuilder : IBuilder
        {
            TNestedBuilder builder = BuildUsing<TNestedBuilder>();
            if (opts != null)
                builder.Setup(opts); //opts(builder);
            SetPropertyBuilder<TNestedBuilder>(prop, builder);
        }

        /// <summary>
        /// Instructs the builder that the specified key store the specified nested builder, with optional settings on the nested builder.
        /// </summary>
        /// <typeparam name="TNestedBuilder"></typeparam>
        /// <param name="key"></param>
        /// <param name="opts"></param>
        protected void SetProperty<TNestedBuilder>(string key, Action<TNestedBuilder> opts = null) where TNestedBuilder : IBuilder
        {
            TNestedBuilder builder = BuildUsing<TNestedBuilder>();
            if (opts != null)
                builder.Setup(opts);//opts(builder);
            SetPropertyBuilder<TNestedBuilder>(key, builder);
        }
        

        protected void SetCollection<TChild, TChildBuilder>(
            Expression<Func<TSubject, object>> prop,
            Action<CollectionBuilder<TChild, TChildBuilder>> opts)
            where TChild : class
            where TChildBuilder : Builder<TChild>
        {
            var colBuilder = new CollectionBuilder<TChild, TChildBuilder>(this);
            opts(colBuilder);
            SetPropertyBuilder(prop, colBuilder);
        }

        protected CollectionBuilder<TChild, TChildBuilder> GetCollection<TChild, TChildBuilder>(Expression<Func<TSubject, object>> prop)
            where TChild : class
            where TChildBuilder : Builder<TChild>
        {
            CollectionBuilder<TChild, TChildBuilder> col = null;
            string key = GetPropertyName(prop);
            if (PropertyBuilders.ContainsKey(key))
            {
                col = PropertyBuilders[key] as CollectionBuilder<TChild, TChildBuilder>;
            }
            if (col == null)
                col = new CollectionBuilder<TChild, TChildBuilder>(this);

            return col;
        }

        private void SetPropertyBuilder<TNestedBuilder>(Expression<Func<TSubject, object>> prop, TNestedBuilder builder) where TNestedBuilder : IBuilder
        {
            SetPropertyBuilder(GetPropertyName(prop), builder);
        }

        private void SetPropertyBuilder<TNestedBuilder>(string key, TNestedBuilder builder) where TNestedBuilder : IBuilder
        {
            if (PropertyBuilders.ContainsKey(key))
                PropertyBuilders.Remove(key);
            PropertyBuilders.Add(key, builder);
        }

        /// <summary>
        /// Checks if property value (or builder that returns a value) has been registered for the specified property.
        /// </summary>
        /// <typeparam name="T">The property's type.</typeparam>
        /// <param name="prop">Lambda expression pointing out the property.</param>
        /// <returns>True if an opt-in exists, otherwise, false.</returns>
        protected bool HasProperty<T>(Expression<Func<TSubject, T>> prop)
        {
            MemberExpression member = (MemberExpression)prop.Body;
            string key = member.Member.Name;
            return HasProperty(key);
        }

        [Obsolete("Replace with HasProperty, HasOptInFor will be removed in version 1.0.")]
        protected bool HasOptInFor<T>(Expression<Func<TSubject, T>> prop)
        {
            return HasProperty(prop);
        }

        /// <summary>
        /// Checks if property value (or builder that returns a value) has been registered for the specified key.
        /// </summary>
        /// <param name="key">Key to check.</param>
        /// <returns>True if a property value can be resolved, otherwise, false.</returns>
        protected bool HasProperty(string key)
        {
            if (PropertyBuilders.ContainsKey(key))
                return true;
            return false;
        }

        [Obsolete("Replace with HasProperty, HasOptInFor will be removed in version 1.0.")]
        protected bool HasOptInFor(string key)
        {
            return HasProperty(key);
        }

        /// <summary>
        /// Gets the builder associated with the specified property.
        /// </summary>
        /// <typeparam name="T">Property type</typeparam>
        /// <param name="prop">Lamnda expression pointing to the property.</param>
        /// <param name="orUse">Func that returns a default if no builder was registered for the specified property.</param>
        /// <returns></returns>
        protected TBuilder GetPropertyBuilder<TBuilder>(Expression<Func<TSubject, object>> prop, Func<TBuilder> orUse)
        {
            string key = GetPropertyName(prop);
            if (PropertyBuilders.ContainsKey(key))
                return (TBuilder)PropertyBuilders[key];

            return orUse();
        }

        /// <summary>
        /// Gets the value/instance set for the specified key.
        /// </summary>
        /// <typeparam name="T">Type of the value/instance.</typeparam>
        /// <param name="key">Key for the value/instance</param>
        /// <param name="orUse">Func that returns a default value if nothing was registered for the specified key.</param>
        /// <returns></returns>
        protected T GetProperty<T>(string key, Func<T> orUse)
        {
            if (!PropertyBuilders.ContainsKey(key))
                return orUse();
            return (T)PropertyBuilders[key].Create();
        }

        [Obsolete("Replace with GetProperty, OptInFor will be removed in version 1.0.")]
        protected T OptInFor<T>(string key, Func<T> valueIfNoOptIn)
        {
            return GetProperty(key, valueIfNoOptIn);
        }

        /// <summary>
        /// Gets the value/instance set for the specified key.
        /// </summary>
        /// <typeparam name="T">Type of the value/instance.</typeparam>
        /// <param name="key">Key for the value/instance</param>
        /// <param name="orUse">Value or instance to return if nothing was registered for the specified key.</param>
        /// <returns></returns>
        protected T GetProperty<T>(string key, T orUse)
        {
            return GetProperty(key, () => orUse);
        }

        [Obsolete("Replace with GetProperty, OptInFor will be removed in version 1.0.")]
        protected T OptInFor<T>(string key, T valueIfNoOptIn)
        {
            return GetProperty(key, valueIfNoOptIn);
        }

        /// <summary>
        /// Gets the value/instance set for the specified property.
        /// </summary>
        /// <typeparam name="T">Type of the property</typeparam>
        /// <param name="prop">Lambda expression pointing to the property</param>
        /// <param name="orUse">Func that returns a default if nothing was registered for the property.</param>
        /// <returns></returns>
        protected T GetProperty<T>(Expression<Func<TSubject, T>> prop, Func<T> orUse)
        {
            return GetProperty(GetPropertyName(prop), orUse);
        }

        [Obsolete("Replace with GetProperty, OptInFor will be removed in version 1.0.")]
        protected T OptInFor<T>(Expression<Func<TSubject, T>> prop, Func<T> valueIfNoOptIn)
        {
            return GetProperty(prop, valueIfNoOptIn);
        }

        /// <summary>
        /// Gets the value/instance set for the specified property.
        /// </summary>
        /// <typeparam name="T">Type of the property</typeparam>
        /// <param name="prop">Lambda expression pointing to the property</param>
        /// <param name="orUse">Value or instance to return if nothing was registered for the property.</param>
        /// <returns></returns>
        protected T GetProperty<T>(Expression<Func<TSubject, T>> prop, T orUse)
        {
            return GetProperty(prop, () => orUse);
        }

        [Obsolete("Replace with GetProperty, OptInFor will be removed in version 1.0.")]
        protected T OptInFor<T>(Expression<Func<TSubject, T>> prop, T valueIfNoOptIn)
        {
            return GetProperty(prop, valueIfNoOptIn);
        }

        /// <summary>
        /// Adds a customization to the builder that will be applied to the instance being built. Will be applied as the last step in building the instance.
        /// </summary>
        /// <param name="action">Customization to apply to the built instance.</param>
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
            TSubject subject;
            if (Instance != null)
                subject = Instance;
            else
            {
                foreach (Action setup in Setups)
                {
                    setup();
                }
                subject = Build(seed);
            }
                
            foreach (var cust in Customizations)
                cust(subject);
            
            return subject;
        }

        object IBuilder.Create(int seed)
        {
            return Create(seed);
        }

        /// <summary>
        /// Implicit operator, enabling you to say MyType = new MyTypeBuilder(); without .Create(), which becomes implicit.
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Builds an instance using this builder
        /// </summary>
        /// <param name="seed"></param>
        /// <returns></returns>
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

        private string GetPropertyName<T>(Expression<Func<TSubject, T>> expr)
        {
            MemberExpression member = expr.Body as MemberExpression;
            if (member != null)
                return member.Member.Name;

            UnaryExpression unary = expr.Body as UnaryExpression;
            if (unary != null)
            {
                MemberExpression unaryMember = unary.Operand as MemberExpression;
                if (unaryMember != null)
                    return unaryMember.Member.Name;
            }

            throw new ArgumentException(String.Format("The property expression should point out a member of the class {0}, however the expression does not seem to do so.", typeof(TSubject).Name));
        }
    }
}