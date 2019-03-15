using System;

namespace DomainModel
{
    public class Regularization
    {
        private int _employeeID { get; set; }
        private DateTime _regularizedDate { get; set; }
        private TimeSpan _reguralizedHours { get; set; }
        private string _remark { get; set; }

        public Regularization(int employeeID,DateTime regularizedDate,TimeSpan reguralizedHours, string remark)
        {
            _employeeID = employeeID;
            _regularizedDate = regularizedDate;
            _reguralizedHours = reguralizedHours;
            _remark = remark;
        }

        public TimeSpan GetRegularizedHours()
        {
            return _reguralizedHours;
        }

        public string GetRemark()
        {
            return _remark;
        }

        public DateTime RegularizedDate()
        {
            return _regularizedDate;
        }
    }
}
