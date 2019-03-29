using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace DomainModel
{
    public class Holiday
    {
        private DateTime _date;
        private string _name;

        public Holiday(DateTime date, string name)
        {
            _date = date;
            _name = name;
        }

        public DateTime Date()
        {
            return _date;
        }

        public string Name()
        {
            return _name;
        }
    }
}
