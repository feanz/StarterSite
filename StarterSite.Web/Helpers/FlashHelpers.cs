using System;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace StarterSite.Web.Helpers
{
    /// <summary>
    /// Ubiquitous site wide messaging system
    /// </summary>
    public static class FlashHelpers
    {
        public static void FlashInfo(this Controller controller, string message)
        {
            controller.TempData["info"] = message;
        }

        public static void FlashWarning(this Controller controller, string message)
        {
            controller.TempData["warning"] = message;
        }

        public static IHtmlString Flash(this HtmlHelper helper)
        {
            var message = "";
            var className = "";
            if (helper.ViewContext.TempData["info"] != null)
            {
                message = helper.ViewContext.TempData["info"].ToString();
                className = "notification-info";
            }
            else if (helper.ViewContext.TempData["warning"] != null)
            {
                message = helper.ViewContext.TempData["warning"].ToString();
                className = "notification-warning";
            }
           
            //remove invalide chars
            message = message.Replace("'", "");

            var sb = new StringBuilder();
            if (!String.IsNullOrEmpty(message))
            {
                sb.AppendLine("<script>");
                sb.AppendLine("$(document).ready(function() {");
                sb.AppendFormat("$('#flash').html('{0}');", message);                
                sb.AppendFormat("$('#flash').toggleClass('{0}');", className);
                sb.AppendFormat("$('#flash').toggleClass('{0}');", "box-shadow");
                sb.AppendLine("$('#flash').fadeIn(3000).delay(1000);");                
                sb.AppendLine("$('#flash').click(function(){$('#flash').fadeOut('slow');});");
                sb.AppendLine("});");
                sb.AppendLine("</script>");
            }
            return new HtmlString(sb.ToString());
        }

    }
}
