namespace FluentBuilders.Core
{
    /// <summary>
    /// Simple interface for a builder factory.
    /// </summary>
    public interface IBuilderFactory
    {
        T Create<T>() where T : IBuilder;
    }
}
