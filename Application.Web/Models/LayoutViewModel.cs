using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Application.Web.Models
{
    public enum Visibility
    {
        none,
        block
    }

    public class LayoutViewModel
    {
        public LayoutViewModel()
        {
            VisibilityReporteesTab = Visibility.none.ToString();
        }
        [BindProperty]
        public string VisibilityReporteesTab { get; set; }

    }
}