using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using StarterSite.Web.Helpers;

namespace StarterSite.Web
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            //Use attribute based routing see action methods decorated with Route attribute
            RouteAttribute.MapDecoratedRoutes(routes);

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
            );

        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);

            SeedUsersAndRoles();
        }

        private static void SeedUsersAndRoles()
        {
            if(!Roles.RoleExists("Admin"))
            {
                Roles.CreateRole("Admin");
            }
            if (!Roles.RoleExists("SuperUser"))
            {
                Roles.CreateRole("SuperUser");
            }
            if (!Roles.RoleExists("User"))
            {
                Roles.CreateRole("User");
            }
            if(Membership.GetUser("richard.a.forrest@gmail.com") == null)
            {
                Membership.CreateUser("richard.a.forrest@gmail.com", "Password1", "richard.a.forrest@gmail.com");
            }
            if (!Roles.IsUserInRole("richard.a.forrest@gmail.com", "Admin"))
            {
                Roles.AddUserToRole("richard.a.forrest@gmail.com", "Admin");
            }
        }
    }
}