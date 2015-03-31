namespace FluentBuilders.Core
{
    public interface IBuilderFactory
    {
        T Create<T>() where T : IBuilder;
    }
}
