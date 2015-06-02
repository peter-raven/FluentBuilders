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

        public new void SetProperty<T>(Expression<Func<TSubject, object>> prop, T instance)
        {
            base.SetProperty(prop, instance);
        }

        public new void SetProperty<T>(string key, T instance)
        {
            base.SetProperty(key, instance);
        }

        public new void SetProperty<TNestedBuilder>(Expression<Func<TSubject, object>> prop, Action<TNestedBuilder> opts = null) where TNestedBuilder : IBuilder
        {
            base.SetProperty(prop, opts);
        }

        public new void SetProperty<TNestedBuilder>(string key, Action<TNestedBuilder> opts = null) where TNestedBuilder : IBuilder
        {
            base.SetProperty(key, opts);
        }

        public new bool HasProperty<T>(Expression<Func<TSubject, T>> prop)
        {
            return base.HasProperty(prop);
        }

        public new bool HasProperty(string key)
        {
            return base.HasProperty(key);
        }

        public new T GetProperty<T>(string key, Func<T> valueIfNoOptIn)
        {
            return base.GetProperty(key, valueIfNoOptIn);
        }

        public new T GetProperty<T>(string key, T valueIfNoOptIn)
        {
            return base.GetProperty(key, valueIfNoOptIn);
        }

        public new T GetProperty<T>(Expression<Func<TSubject, T>> prop, Func<T> valueIfNoOptIn)
        {
            return base.GetProperty(prop, valueIfNoOptIn);
        }

        public new T GetProperty<T>(Expression<Func<TSubject, T>> prop, T valueIfNoOptIn)
        {
            return base.GetProperty(prop, valueIfNoOptIn);
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