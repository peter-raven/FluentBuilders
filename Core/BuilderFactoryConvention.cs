using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BuildBuddy.Core
{
    public class BuilderFactoryConvention
    {
        private IBuilderFactory _factory;

        public BuilderFactoryConvention()
        {
            _factory = new SimpleBuilderFactory();
        }

        public void UseFactory(IBuilderFactory factory)
        {
            _factory = factory;
        }

        internal T Create<T>() where T : IBuilder
        {
            T builder = _factory.Create<T>();
            builder.BuilderFactoryConvention.UseFactory(_factory);
            return builder;
        }
    }
}
