namespace BuildBuddy.Core
{
    public interface IBuilder
    {
        T BuildUsing<T>() where T : IBuilder;
        object Create(int seed = 0);
        BuilderFactoryConvention BuilderFactoryConvention { get; set; }
    }

    public interface IBuilder<T> : IBuilder
    {
        new T Create(int seed = 0);
    }
}