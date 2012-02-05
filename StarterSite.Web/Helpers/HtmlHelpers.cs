using System.Security.Principal;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace StarterSite.Web.Helpers
{
    public static class HtmlHelpers
    {
        /// <summary>
        /// Output Menu UL based on the contents of default Sitemap. Menu supports permission based filtering 
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="user"></param>
        /// <param name="menuItem"></param>
        /// <returns></returns>
        public static IHtmlString Menu(this HtmlHelper helper, IPrincipal user, string menuItem = "")
        {
            var sb = new StringBuilder();

            // Create opening unordered items tag
            sb.Append("<ul id='menu'>");

            // Render each top level node            
            var topLevelNodes = SiteMap.RootNode.ChildNodes;
            var count = 1;
            foreach (SiteMapNode node in topLevelNodes)
            {
                //If the user has permissions to see this menu item or they are a developer (developers see all)
                if ((node.Roles.Count == 0) || user.IsInRole(node.Roles[0].ToString()) || user.IsInRole("Admin"))
                {
                    sb.AppendLine("<li>");

                    //If a menu item has been provided check if the node contains the menuitem and mark as selected
                    if (!string.IsNullOrEmpty(menuItem))
                    {
                        sb.AppendFormat(
                            node.Title == menuItem
                                ? "<a href='{0}' class='selectedMenuItem'>{1}</a>"
                                : "<a href='{0}'>{1}</a>", node.Url, helper.Encode(node.Title));
                    }
                    //If there is no current node make first node selected
                    else if (SiteMap.CurrentNode == null && count == 1)
                        sb.AppendFormat("<a href='{0}' class='selectedMenuItem'>{1}</a>", node.Url, helper.Encode(node.Title));
                    else
                        sb.AppendFormat("<a href='{0}'>{1}</a>", node.Url, helper.Encode(node.Title));

                    sb.AppendLine("</li>");
                }

                count++;
            }

            // Close unordered items tag
            sb.Append("</ul>");

            return new HtmlString(sb.ToString());
        }
    }
}