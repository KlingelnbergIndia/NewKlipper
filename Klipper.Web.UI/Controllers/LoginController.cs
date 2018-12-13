using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Klipper.Web.Application.Login;
using Klipper.Web.UI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Klipper.Web.UI.Controllers
{
    public class LoginController : Controller
    {
        private readonly IAuthenticate _auth;
        public LoginController(IAuthenticate auth)
        {
            _auth = auth;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Authenticate([FromForm] LoginViewModel login)
        {
            _auth.Login(login.UserName,login.Password);
            if (_auth.ResponseStatus== LoginResponse.Success)
            {
                HttpContext.Session.SetString("UserName", login.UserName);
                return RedirectToAction("Index", "Home");
            }

            HttpContext.Session.Clear();
            TempData["errorMessage"] = _auth.ResponseMessage;
            return RedirectToAction("Index");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            TempData["successMessage"] = "Logged out!";
            return RedirectToAction("Index");
        }
    }
}