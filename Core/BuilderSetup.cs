using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace BuildBuddy.Core
{
    public abstract class BuilderSetup<TBuilder, TSubject> : BuilderSetupBase
        where TSubject : class
        where TBuilder : class
    {
        protected readonly List<Action<TSubject>> Customizations;

        protected BuilderSetup()
        {
            Customizations = new List<Action<TSubject>>();
        }
        
        public TBuilder Customize(Action<TSubject> action)
        {
            Customizations.Add(action);
            return this as TBuilder;
        }

        public virtual TSubject Create(int seed = 0)
        {
            TSubject subject = Build(seed);
            foreach (var cust in Customizations)
                cust(subject);
            
            return subject;
        }

        public static implicit operator TSubject(BuilderSetup<TBuilder, TSubject> builder)
        {
            return builder.Create();
        }

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

        protected T BuildUsing<T>() where T : BuilderSetupBase
        {
            return BuilderManager.ConstructUsing<T>();
        }
    }
}