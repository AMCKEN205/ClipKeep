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
            // Set properties we need for DB connection on application startup.
            CosmosConfig.InitCosmosConfig();
        }
        
    }
}
