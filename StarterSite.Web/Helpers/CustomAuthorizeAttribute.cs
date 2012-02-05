using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace StarterSite.Web.Helpers
{
    /// <summary>
    /// Override the standard AuthorizeAttribute with a Custom implmentation that uses are custom Identity and 
    /// Principal to authenticate users actions 
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
    public sealed class CustomAuthorizeAttribute : AuthorizeAttribute
    {
        /// <summary>
        /// Uses Custom Identity and Principal to check that user is validated for this system and if the user is in the role 
        /// specified on the auth attribute.
        /// </summary>
        /// <param name="httpContext"></param>
        /// <returns></returns>
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            var authorised = false;

            if (httpContext.User.Identity.IsAuthenticated)
            {
                if (httpContext.User.IsInRole(Roles))
                {
                    authorised = true;
                }
            }
            return authorised;
        }

        /// <summary>
        /// Cverride the standard Authorize attribute.  Checks that user is authenticated and has valide permissions
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext == null)
            {
                throw new ArgumentNullException("filterContext");
            }
            if (AuthorizeCore(filterContext.HttpContext))
            {
                SetCachePolicy(filterContext);
            }
            else if (ActionAllowAnonymousAccess(filterContext))
            {
                SetCachePolicy(filterContext);
            }
            else if (!filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                // auth failed, redirect to no access page
                filterContext.Result =
                    new RedirectToRouteResult(
                        (new RouteValueDictionary { { "controller", "Error" }, { "action", "NoAccess" } }));

            }
            else if (filterContext.HttpContext.User.IsInRole("Admin"))
            {
                // is authenticated and is in the Admin role
                SetCachePolicy(filterContext);
            }
            else
            {
                // auth failed, redirect to Insufficient Permissions
                filterContext.Result = new RedirectToRouteResult((new RouteValueDictionary { { "controller", "Error" }, { "action", "InsufficientPermissions" } }));
            }
        }

        private static bool ActionAllowAnonymousAccess(AuthorizationContext filterContext)
        {
            return (filterContext.ActionDescriptor.GetCustomAttributes(typeof(AllowAnonymousAttribute), true).FirstOrDefault() as AllowAnonymousAttribute) != null;
        }

        /// <summary>
        /// ** IMPORTANT **
        /// Since we're performing authorization at the action level, the authorization code runs
        /// after the output caching module. In the worst case this could allow an authorized user
        /// to cause the page to be cached, then an unauthorized user would later be served the
        /// cached page. We work around this by telling proxies not to cache the sensitive page,
        /// then we hook our custom authorization code into the caching mechanism so that we have
        /// the final say on whether a page should be served from the cache.
        /// </summary>
        /// <param name="filterContext"></param>
        private void SetCachePolicy(AuthorizationContext filterContext)
        {
            var cachePolicy = filterContext.HttpContext.Response.Cache;
            cachePolicy.SetProxyMaxAge(new TimeSpan(0));
            cachePolicy.AddValidationCallback(CacheValidateHandler, null /* data */);
        }

        private void CacheValidateHandler(HttpContext context, object data, ref HttpValidationStatus validationStatus)
        {
            validationStatus = OnCacheAuthorization(new HttpContextWrapper(context));
        }
    }
}