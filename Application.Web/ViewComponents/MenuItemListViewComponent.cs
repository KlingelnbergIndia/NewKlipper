using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Microsoft.AspNetCore.Hosting.Internal.HostingApplication;

namespace Application.Web.ViewComponents
{
    public class MenuItemListViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            string empRolesJson = HttpContext.Session.GetString("EmployeeRoles");
            var menuList = new List<MenuLinksViewModel>();

            if (empRolesJson != null)
            {
                empRolesJson = string.IsNullOrEmpty(empRolesJson) ? "" : empRolesJson;
                var EmployeeRolesList = JsonConvert.DeserializeObject<string[]>(empRolesJson);
                menuList = GetMenuItemsForRole(EmployeeRolesList);
            }

            ViewBag.MenuList = menuList;
            ViewBag.EmployeeName = HttpContext.Session.GetString("EmployeeName");
            ViewBag.EmployeeTitle = HttpContext.Session.GetString("Title");

            return await Task.Run(() =>
            {
                return View();
            });
        }

        private List<MenuLinksViewModel> GetMenuItemsForRole(string[] employeeRolesList)
        {
            var menuList = new List<MenuLinksViewModel>();
            
            if (employeeRolesList.Contains("TeamLeader"))
            {
                menuList.Add(new MenuLinksViewModel
                {
                    IndexPosition = 2,
                    Label = "Reportees",
                    Controller = "Home",
                    Action = "Reportees",
                    Glyphicon = "glyphicon glyphicon-user"

                });
            }
            menuList.Add(new MenuLinksViewModel
            {
                IndexPosition = 1,
                Label = "Summary",
                Controller = "Home",
                Action = "Index",
                Glyphicon = "glyphicon glyphicon-duplicate"
            });

            menuList.Add(new MenuLinksViewModel
            {
                IndexPosition = 10,
                Label = "Logout",
                Controller = "Login",
                Action = "Logout",
                Glyphicon = "glyphicon glyphicon-off"

            });

            return menuList
                .OrderBy(x=>x.IndexPosition)
                .ToList();
        }
    }

    public class MenuLinksViewModel
    {
        public int IndexPosition { get; set; }
        public string Label { get; set; }
        public string Controller { get; set; }
        public string Action { get; set; }
        public string Glyphicon { get; set; }
    }
}
