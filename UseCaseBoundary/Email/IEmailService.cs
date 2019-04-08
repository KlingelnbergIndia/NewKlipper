using System;
using System.Collections.Generic;
using System.Text;
using DomainModel;

namespace UseCaseBoundary.Email
{
    public interface IEmailService
    {
        void SendMailForAddNewLeave(Leave takenLeave);
    }
}
