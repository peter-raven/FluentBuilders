using System;

namespace FluentBuilders.Core
{
    /// <summary>
    /// A builder that can be persisted to storage.
    /// </summary>
    /// <typeparam name="TSubject">Type of object built by this builder</typeparam>
    public abstract class PersistingBuilder<TSubject> : Builder<TSubject>, IPersistingBuilder
        where TSubject : class
    {
        /// <summary>
        /// Set to true to persist the object when built.
        /// </summary>
        public bool Persist { get; set; }

        /// <summary>
        /// Register an action to perform on the built instance just before persisting it.
        /// </summary>
        /// <param name="action">Action to perform.</param>
        /// <returns></returns>
        public new PersistingBuilder<TSubject> Customize(Action<TSubject> action)
        {
            Customizations.Add(action);
            return this;
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

        /// <summary>
        /// Saves the created instance to persistent storage.
        /// </summary>
        /// <param name="subject">Instance to persist.</param>
        protected abstract void Save(TSubject subject);

        /// <summary>
        /// Override this method to perform actions on the created instance just after it has been saved to persistent storage.
        /// </summary>
        /// <param name="subject"></param>
        protected virtual void PostPersist(TSubject subject)
        {
        }
    }
}