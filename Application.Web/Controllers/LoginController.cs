using System.Collections.Generic;
using System.Linq;
using Application.Web.Models;
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
        private IAuthMongoDBRepository _authMongoDbRepository;

        public LoginController(IEmployeeRepository employeeRepository,
            IAuthMongoDBRepository authMongoDbRepository)
        {
            _employeeRepository = employeeRepository;
            _authMongoDbRepository = authMongoDbRepository;
        }

        public IActionResult Login()
        {
            return View();
        }

        public IActionResult Authenticate([FromForm] LoginViewModel login)
        {
            var userLogin = new Login(_employeeRepository,
                _authMongoDbRepository);

            var loggedInUserDetails = userLogin.LoginUser(
                login.UserName, 
               login.Password);

            DisplayOfReporteeTab(loggedInUserDetails?.Roles());

            if (loggedInUserDetails != null)
            {
                HttpContext.Session.SetString
                    ("EmployeeName", 
                    $"{loggedInUserDetails.FirstName()} {loggedInUserDetails.LastName()}");
                HttpContext.Session.SetString
                    ("Title", loggedInUserDetails.Title());
                HttpContext.Session.SetInt32
                    ("ID", loggedInUserDetails.Id());
                HttpContext.Session.SetString
                    ("EmployeeRoles", setEmployeeRolesJson(loggedInUserDetails.Roles()));

                return RedirectToAction("Index", "Home");
            }
            HttpContext.Session.Clear();
            TempData["errorMessage"] = "Invalid username or password";
            return RedirectToAction("Login");
        }

        private void DisplayOfReporteeTab(List<EmployeeRoles> roles)
        {
            string VisibilityOfReporteesTab;
            VisibilityOfReporteesTab = Employee.CanContainReportees(roles) ? "block" : "none";
            HttpContext.Session.SetString("VisibilityOfReporteesTab", VisibilityOfReporteesTab);
        }

        private string setEmployeeRolesJson(List<EmployeeRoles> list)
        {
            var emploeeListStrings = list.Select(x => x.ToString()).ToList();
            var rolesJson = JsonConvert.SerializeObject(emploeeListStrings);
            return rolesJson;
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            TempData["successMessage"] = "Logged out!";
            return RedirectToAction("Login");
        }

    }
}