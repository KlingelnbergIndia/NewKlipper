using DataAccess;
using DomainModel;
using System;
using System.Collections.Generic;
using System.Text;
using DataAccess.EntityModel.Holiday;
using MongoDB.Driver;
using UseCaseBoundary;

namespace RepositoryImplementation
{
    public class CompanyHolidayMongoDBRepository: ICompanyHolidayRepository
    {
        private readonly HolidaysDBContext _context = null;

        public CompanyHolidayMongoDBRepository()
        {
            _context = HolidaysDBContext.Instance;
        }

        public List<Holiday> Holidays()
        {
           var listOfEntityModel = _context.holidays.Find(_ => true).ToList();
           var listOfDomainModel = new List<Holiday>();

           foreach (var entityModel in listOfEntityModel)
           {
               listOfDomainModel
                   .Add(new Holiday(entityModel.Date, entityModel.Holiday));
           }

           return listOfDomainModel;
        }
    }
}
