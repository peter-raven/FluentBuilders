namespace BuildBuddy.Core
{
    public interface IBuilderManager
    {
        T BuildUsing<T>();
    }
}
