using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace Application.Web.Controllers
{
    //[MyActionFilter]
    //public class ApplicationController : Controller
    //{
    //    public LayoutViewModel layoutViewModel { get; set; }

    //    public ApplicationController()
    //    {
    //        this.layoutViewModel = new LayoutViewModel();
    //        this.ViewData["LayoutViewModel"] = this.layoutViewModel;
    //    }

    //}

    [MyActionFilter]
    public class ApplicationController : Controller
    {
        public LayoutViewModel _layoutViewModel { get; }

        public ApplicationController(LayoutViewModel layoutViewModel)
        {
            //this._layoutViewModel = layoutViewModel;
            //this.ViewData["LayoutViewModel"] = this._layoutViewModel;
        }

    }
}