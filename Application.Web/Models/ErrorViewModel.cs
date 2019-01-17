using System;

namespace Application.Web.Models
{
    public class ErrorViewModel : LayoutViewModel
    {
        public string RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}