namespace BuildBuddy.Core
{
    public interface IBuilder
    {
        T BuildUsing<T>() where T : IBuilder;
    }
}