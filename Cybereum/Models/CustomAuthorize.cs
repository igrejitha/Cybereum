using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Cybereum.Models
{
    public class CustomAuthorize: AuthorizeAttribute
    {
        
        public CustomAuthorize(params string[] roles)
        {
            Roles = String.Join(",", roles);
        }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            bool authorized = false;

            foreach (var role in this.Roles)
                if (HttpContext.Current.User.IsInRole(role.ToString()))
                {
                    authorized = true;
                    break;
                }

            if (!authorized)
            {
                var url = new UrlHelper(filterContext.RequestContext);
                var logonUrl = url.Action("Http", "Error", new { Id = 401, Area = "" });
                filterContext.Result = new RedirectResult(logonUrl);

                return;
            }

            if (!filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                //if not logged, it will work as normal Authorize and redirect to the Login
                base.HandleUnauthorizedRequest(filterContext);

            }
            else
            {
                //logged and wihout the role to access it - redirect to the custom controller action
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = "Error", action = "AccessDenied" }));
            }
        }
    }
}