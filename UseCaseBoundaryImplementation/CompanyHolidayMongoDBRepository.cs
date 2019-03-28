using DataAccess;
using System;
using System.Collections.Generic;
using System.Text;

namespace RepositoryImplementation
{
    public class CompanyHolidayMongoDBRepository
    {
        private readonly AttendanceDBContext _context = null;

        public CompanyHolidayMongoDBRepository()
        {
            _context = AttendanceDBContext.Instance;
        }

    }
}
