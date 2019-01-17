using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Web.PageAccessAuthentication
{
    public class AuthenticateTeamLeaderRole : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            string user = filterContext.HttpContext.Session.GetString("EmployeeRoles");
            var rolesJson = string.IsNullOrEmpty(user) ? "" : user;
            var EmployeeRolesList = JsonConvert.DeserializeObject<string[]>(rolesJson);

            if (!EmployeeRolesList.Contains("TeamLeader"))
                filterContext.Result = new Http403Result();


        }
    }

    internal class Http403Result : IActionResult
    {
        public Task ExecuteResultAsync(ActionContext context)
        {
            context.HttpContext.Response.StatusCode = 403;
            return Task.FromResult(context);
        }
    }
}


