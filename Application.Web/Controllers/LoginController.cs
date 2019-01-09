﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using Application.Web.Models;
using Application.Web.PageAccessAuthentication;
using DomainModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using UseCaseBoundary;
using UseCases;

namespace Application.Web.Controllers
{
    public class LoginController : Controller
    {
        private IEmployeeRepository _employeeRepository;
        public LoginController(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }
        public IActionResult Login()
        {
            return View();
        }

        public IActionResult Authenticate([FromForm] LoginViewModel login)
        {
            Login userLogin = new Login(_employeeRepository);
            var loggedInUserDetails = userLogin.LoginUser(
                login.UserName, 
                ToSha256(login.Password));

            if (loggedInUserDetails != null)
            {
                HttpContext.Session.SetString("EmployeeName", $"{loggedInUserDetails.FirstName()} {loggedInUserDetails.LastName()}");
                HttpContext.Session.SetString("Title", loggedInUserDetails.Title());
                HttpContext.Session.SetInt32("ID", loggedInUserDetails.Id());
                HttpContext.Session.SetString("EmployeeRoles", setEmployeeRolesJson(loggedInUserDetails.Roles()));

                return RedirectToAction("Index", "Home");
            }
            HttpContext.Session.Clear();
            TempData["errorMessage"] = "Invalid username or password";
            return RedirectToAction("Login");
        }

        private string setEmployeeRolesJson(List<EmployeeRoles> list)
        {
            var dat = list.Select(x => x.ToString()).ToList();
            var rolesJson = JsonConvert.SerializeObject(dat);
            return rolesJson;
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            TempData["successMessage"] = "Logged out!";
            return RedirectToAction("Login");
        }

        private static string ToSha256(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return string.Empty;

            using (var sha = SHA256.Create())
            {
                var bytes = System.Text.Encoding.ASCII.GetBytes(input);
                var hash = sha.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }
    }
}