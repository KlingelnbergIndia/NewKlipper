using DomainModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace UseCaseBoundary
{
    public interface ICompanyHolidayRepository
    {
        List<Holiday> Holidays();
    }
}
