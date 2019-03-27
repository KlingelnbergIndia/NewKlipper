using System;
using System.Collections.Generic;
using System.Text;

namespace DomainModel
{
    public class User
    {
        public int _id;
        private string _userName;
        private string _password;
        private string _emailId;

        public int UserId()
        {
            return _id;
        }
    }
}
