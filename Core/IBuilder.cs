using System;
using System.Collections.Generic;

namespace FluentBuilders.Core
{
    public interface IBuilder
    {
        T BuildUsing<T>() where T : IBuilder;
        object Create(int seed = 0);
        BuilderFactoryConvention BuilderFactoryConvention { get; set; }
        List<Action> Setups { get; }
    }

    public interface IBuilder<T> : IBuilder
    {
        new T Create(int seed = 0);
    }
}