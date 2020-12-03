namespace ClipKeep.Models.Interfaces
{
    /// <summary>
    /// Defines methods that should exist within model to be stored in the Cosmos DB
    /// </summary>
    public interface IDisplayable
    {
        /// <summary>
        /// Display the object on the frontend webpage.
        /// </summary>
        void Display();

    }
}