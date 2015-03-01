namespace BuildBuddy.Core
{
    public interface IBuilderFactory
    {
        T Create<T>();
    }
}
