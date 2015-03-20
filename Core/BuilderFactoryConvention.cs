using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FluentBuilders.Core
{
    /// <summary>
    /// Keeps the convention for creating new builders.
    /// </summary>
    public class BuilderFactoryConvention
    {
        private IBuilderFactory _factory;

        /// <summary>
        /// Default constructor. Sets the current convention to use a SimpleBuilderFactory.
        /// </summary>
        public BuilderFactoryConvention()
        {
            _factory = new SimpleBuilderFactory();
        }

        /// <summary>
        /// Change the convention to use the specified factory for creating new builders.
        /// </summary>
        /// <param name="factory"></param>
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
