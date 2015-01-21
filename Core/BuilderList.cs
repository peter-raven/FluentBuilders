using System;
using System.Collections.Generic;
using System.Linq;

namespace BuildBuddy.Core
{
    public class BuilderList<TItem, TFactory> : List<BuilderTuple<TItem, TFactory>>
        where TItem : class
        where TFactory : BuilderSetup<TFactory, TItem>
    {
        public int CreatorsWithReadyInstanceCount()
        {
            return this.Count(x => x.Instance != null);
        }

        public int CreatorsUsingFactoryCount()
        {
            return this.Count(x => x.Factory != null);
        }

        public void AppendResolvedTo(ICollection<TItem> col, Action<TFactory> factoryOpts = null)
        {
            foreach (var item in this)
                col.Add(item.Resolve(factoryOpts));
        }

        public void ForeachResolvedDo(Action<TItem> eachAction)
        {
            foreach (var item in this)
                eachAction(item.Resolve());
        }
    }
}