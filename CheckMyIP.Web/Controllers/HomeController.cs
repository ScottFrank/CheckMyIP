using System;
using System.Web.Mvc;

namespace CheckMyIP.Web.Controllers
{
    /// <summary>
    /// Renders result as JSON and also wraps the JSON in a call
    /// to the callback function specified in "JsonpResult.Callback".
    /// http://stackoverflow.com/questions/4795201/asp-net-mvc-3-jsonp-does-this-work-with-jsonvalueproviderfactory
    /// </summary>
    public class JsonpResult : JsonResult
    {
        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            var request = context.HttpContext.Request;
            var response = context.HttpContext.Response;
            string jsoncallback = (context.RouteData.Values["callback"] as string) ?? request["callback"];
            if (!string.IsNullOrEmpty(jsoncallback))
            {
                if (string.IsNullOrEmpty(base.ContentType))
                {
                    base.ContentType = "application/x-javascript";
                }
                response.Write(string.Format("{0}(", jsoncallback));
            }
            base.ExecuteResult(context);
            if (!string.IsNullOrEmpty(jsoncallback))
            {
                response.Write(")");
            }
        }
    }

    //http://blogorama.nerdworks.in/entry-EnablingJSONPcallsonASPNETMVC.aspx
    public class JsonPFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (filterContext == null)
                throw new ArgumentNullException("filterContext");

            //
            // see if this request included a "callback" querystring parameter
            //
            string callback = filterContext.HttpContext.Request.QueryString["callback"];
            if (callback != null && callback.Length > 0)
            {
                //
                // ensure that the result is a "JsonResult"
                //
                JsonResult result = filterContext.Result as JsonResult;
                if (result == null)
                {
                    throw new InvalidOperationException("JsonpFilterAttribute must be applied only " +
                        "on controllers and actions that return a JsonResult object.");
                }

                filterContext.Result = new JsonpResult
                {
                    ContentEncoding = result.ContentEncoding,
                    ContentType = result.ContentType,
                    Data = result.Data,
                    JsonRequestBehavior = result.JsonRequestBehavior
                };
            }
        }
    }

    [JsonPFilter]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult CheckIP()
        {
            var ip = HttpContext.Request.ServerVariables["REMOTE_ADDR"];

            return Json(new { client_ip = ip }, JsonRequestBehavior.AllowGet);
        }
    }
}
