using System.Collections.Generic;
using System.Threading.Tasks;

namespace ClipKeep.Models.Interfaces
{
    /// <summary>
    /// Define the methods that should be available for models we want to store in the database.
    /// </summary>
    public interface IDbStoreable
    {
        /// <summary>
        /// Store the objects data in the database.
        /// </summary>
        Task<bool> StoreInDb();

    }
}