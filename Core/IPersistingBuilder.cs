namespace FluentBuilders.Core
{
    /// <summary>
    /// A builder that can be persisted to storage.
    /// </summary>
    public interface IPersistingBuilder
    {
        /// <summary>
        /// Set to true to persist the object when built.
        /// </summary>
        bool Persist { get; set; } 
    }
}