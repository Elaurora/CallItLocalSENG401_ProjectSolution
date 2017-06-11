
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace ClientApplicationMVC
{
    public class MvcApplication : HttpApplication
    {
        
        protected void Application_Start()
        {
            

            AreaRegistration.RegisterAllAreas();//Added by VS upon creation of project
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);//Added by VS upon creation of project
            RouteConfig.RegisterRoutes(RouteTable.Routes);//Added by VS upon creation of project
            BundleConfig.RegisterBundles(BundleTable.Bundles);//Added by VS upon creation of project
        }
    }
}
