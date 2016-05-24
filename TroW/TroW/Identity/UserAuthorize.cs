using System.Web.Mvc;
using System.Web.Routing;
using AuthorizeAttribute = System.Web.Mvc.AuthorizeAttribute;

namespace TroW.Identity
{
    public class UserAuthorize : AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (Identity.Current.Customer == null)
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { area = "", controller = "Home", action = "Login" }));
            }
        }
    }

    public class AdminAuthorize : AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (Identity.Current.Customer == null || !Identity.Current.IsAdmin)
            {
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { area = "", controller = "Home", action = "Login" }));
            }
        }
    }
}