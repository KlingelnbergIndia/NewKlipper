using System;
using System.Collections.Generic;
using System.Text;
using UseCaseBoundary.Model;

namespace UseCaseBoundary.DTO
{
    public enum AccessPoint
    {
        MainEntry=16,
        Recreation=19,
        Gymnasium=18
    }
    public class AccessPointRecord
    {
        public Time TimeIn;
        public Time TimeOut;
        public Time TimeSpend;
        public AccessPoint AccessPoint;
    }
}
