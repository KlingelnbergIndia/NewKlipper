using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UseCaseBoundary.DTO;

namespace Application.Web.Models
{
    public class LeaveViewModel
    {
        List<LeaveType> LeaveTypes = GetAllEnumValues();

        private static List<LeaveType> GetAllEnumValues()
        {
            throw new NotImplementedException();
        }
    }
}
