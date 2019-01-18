using System;
using Application.Web.Models;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Application.Web.Controllers
{
    public class MyActionFilter : Attribute, IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            var myController = context.Controller as ApplicationController;

            if (myController != null)
            {
               //myController.layoutViewModel = new LayoutViewModel();

               // myController.ViewBag.LayoutViewModel = myController.layoutViewModel;
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
        }
    }
}