using System;
using System.Collections.Generic;
using System.Linq;

namespace BuildBuddy.Core
{
    public class BuilderList<TItem, TBuilder> : List<BuilderTuple<TItem, TBuilder>>
        where TItem : class
        where TBuilder : Builder<TItem>
    {
        public int CreatorsWithReadyInstanceCount()
        {
            return this.Count(x => x.Instance != null);
        }

        public int CreatorsUsingFactoryCount()
        {
            return this.Count(x => x.Builder != null);
        }

        public void AppendResolvedTo(ICollection<TItem> col, Action<TBuilder> factoryOpts = null)
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