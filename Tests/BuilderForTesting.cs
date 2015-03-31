using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using FluentBuilders.Core;

namespace FluentBuilders.Tests
{
    public class BuilderForTesting<TSubject> : Builder<TSubject> where TSubject : class, new()
    {
        public bool WasAskedToConstruct { get; set; }
        public List<int> WasAskedToConstructWithSeeds { get; private set; }

        public BuilderForTesting()
        {
            WasAskedToConstructWithSeeds = new List<int>();
        }

        public new void OptInWith<T>(Expression<Func<TSubject, object>> prop, T instance)
        {
            base.OptInWith(prop, instance);
        }

        public new void OptInWith<T>(string key, T instance)
        {
            base.OptInWith(key, instance);
        }

        public new void OptInWith<TNestedBuilder>(Expression<Func<TSubject, object>> prop, Action<TNestedBuilder> opts = null) where TNestedBuilder : IBuilder
        {
            base.OptInWith(prop, opts);
        }

        public new void OptInWith<TNestedBuilder>(string key, Action<TNestedBuilder> opts = null) where TNestedBuilder : IBuilder
        {
            base.OptInWith(key, opts);
        }

        public new bool HasOptInFor<T>(Expression<Func<TSubject, T>> prop)
        {
            return base.HasOptInFor(prop);
        }

        public new bool HasOptInFor(string key)
        {
            return base.HasOptInFor(key);
        }

        public new T OptInFor<T>(string key, Func<T> valueIfNoOptIn)
        {
            return base.OptInFor(key, valueIfNoOptIn);
        }

        public new T OptInFor<T>(string key, T valueIfNoOptIn)
        {
            return base.OptInFor(key, valueIfNoOptIn);
        }

        public new T OptInFor<T>(Expression<Func<TSubject, T>> prop, Func<T> valueIfNoOptIn)
        {
            return base.OptInFor(prop, valueIfNoOptIn);
        }

        public new T OptInFor<T>(Expression<Func<TSubject, T>> prop, T valueIfNoOptIn)
        {
            return base.OptInFor(prop, valueIfNoOptIn);
        }

        public TSubject InvokeBuild(int seed)
        {
            return Build(seed);
        }
        
        protected override TSubject Build(int seed)
        {
            WasAskedToConstruct = true;
            WasAskedToConstructWithSeeds.Add(seed);
            var res = new TSubject();

            return res;
        }
    }
}