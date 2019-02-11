using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace Application.Web.Controllers
{
    public class LeaveController : Controller
    {
        public IActionResult Index()
        {
            var leaveViewModel = new LeaveViewModel();
            return View(leaveViewModel);
        }
    }
}