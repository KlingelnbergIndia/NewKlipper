using System;
using System.Collections.Generic;
using System.Text;
using UseCaseBoundary.Model;

namespace UseCaseBoundary.DTO
{
    public class AccessPointRecord
    {
        public Time TimeIn;
        public Time TimeOut;
        public Time TimeSpend;
        public string AccessPoint;
    }
}
