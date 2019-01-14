using System;
using System.Collections.Generic;
using System.Text;

namespace UseCaseBoundary.DTO
{
    public class ReporteeDTO
    {
        public int ID;
        public string FirstName;
        public string LastName;

        public override bool Equals(object other)
        {
            var toCompareWith = other as ReporteeDTO;
            if (toCompareWith == null)
                return false;
            return this.ID == toCompareWith.ID &&
                   this.FirstName == toCompareWith.FirstName &&
                   this.LastName == toCompareWith.LastName;
        }
    }
}
