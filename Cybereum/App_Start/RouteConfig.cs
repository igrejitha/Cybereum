using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Cybereum
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            //routes.MapMvcAttributeRoutes();            
            //"Create/{id}/{taskid}",
            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Account", action = "Login", id = UrlParameter.Optional }
            );
            //routes.MapRoute(
            //    name: "ActivityRoute",
            //    url: "{controller}/{action}/{id}/{taskid}",
            //    defaults: new { controller = "Activity", action = " Create", id = UrlParameter.Optional, taskid = UrlParameter.Optional }
            //);            
        }
    }
}
