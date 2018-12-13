using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Klipper.Web.UI
{
    public class AuthenticateSession : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            string user = filterContext.HttpContext.Session.GetString("UserName");
            if (user == null)
                filterContext.Result = new RedirectResult("/");
        }
    }
}
