using System;
using System.Configuration;

namespace ClipKeep.Models
{
    /// <summary>
    /// Static class used to hold attribute values used to access the applications Cosmos DB.
    /// </summary>
    public static class CosmosConfig
    {
        /// <summary>
        /// The endpoint URL of where our cosmos DB is being hosted
        /// </summary>
        public static string EndPointUrl { get; private set; }
        /// <summary>
        /// The authorization key to allow access to our cosmos DB
        /// </summary>
        public static string AuthorizationKey { get; private set; }
        /// <summary>
        /// The ID of the applications Cosmos DB
        /// </summary>
        public static string DatabaseId { get; private set; }
        /// <summary>
        /// ID of the Users collection. Can think of a collection like a DB table, but unstructured :O!
        /// </summary>
        public static string UsersCollectionId { get; private set; }

        /// <summary>
        /// Initalise properties used to connect to our Cosmos DB.
        /// </summary>
        public static void InitCosmosConfig()
        {
            EndPointUrl = ConfigurationManager.AppSettings["Cosmos.EndPointUrl"];
            AuthorizationKey = ConfigurationManager.AppSettings["Cosmos.AuthorizationKey"];
            DatabaseId = ConfigurationManager.AppSettings["Cosmos.DatabaseId"];
            UsersCollectionId = ConfigurationManager.AppSettings["Cosmos.UsersCollectionId"];
        }

        
    }
}