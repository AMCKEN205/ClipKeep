using System;

namespace ClipKeep.Models
{
    /// <summary>
    /// Static class used to hold attribute values used to access the applications Cosmos DB.
    /// </summary>
    public class CosmosConfig
    {
        public string EndPointUrl { get; set; }
        public string AuthorizationKey { get; set; }
        public string DatabaseId { get; set; }
        public string UsersCollectionId { get; set; }
    }
}