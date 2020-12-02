using System.Configuration;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Helpers;


using ClipKeep.Models;

namespace ClipKeep
{
    public class MvcApplication : HttpApplication
    {
        /// <summary>
        /// Application startup setup
        /// </summary>
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            InitCosmosConfig();
        }

        /// <summary>
        /// Cosmos configuration data, used to access Cosmos DB
        /// </summary>
        public static CosmosConfig CosmosConfig { get; set; }

        /// <summary>
        /// Initalise cosmos config data from root dir web.config file
        /// </summary>
        private static void InitCosmosConfig()
        {
            AntiForgeryConfig.UniqueClaimTypeIdentifier = "name";
            CosmosConfig = new CosmosConfig()
            {
                // ID of the parent database where we are storing our applications containers.
                // Containers can be thought of as individual SQL tables in our use case.
                // (i.e. when using the Cosmos SQL API, however they are unstructured!).
                DatabaseId = ConfigurationManager.AppSettings["Cosmos.DatabaseId"],
                EndPointUrl = ConfigurationManager.AppSettings["Cosmos.EndPointUrl"],
                AuthorizationKey = ConfigurationManager.AppSettings["Cosmos.AuthorizationKey"],
                UsersCollectionId = ConfigurationManager.AppSettings["Cosmos.UsersCollectionId"]
            };
        }
    }
}
