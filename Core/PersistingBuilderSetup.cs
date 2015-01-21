namespace BuildBuddy.Core
{
    public abstract class PersistingBuilderSetup<TBuilder, TSubject> : BuilderSetup<TBuilder, TSubject>
        where TSubject : class
        where TBuilder : class
    {
        protected bool Persist { get; set; }

        public TBuilder Persisted()
        {
            Persist = true;
            return this as TBuilder;
        }

        public override TSubject Create(int seed = 0)
        {
            var subject = base.Create(seed);
            if (Persist)
            {
                Save(subject);
                PostPersist(subject);
            }

            return subject;
        }

        protected abstract void Save(TSubject subject);

        protected virtual void PostPersist(TSubject subject)
        {
        }
    }
}