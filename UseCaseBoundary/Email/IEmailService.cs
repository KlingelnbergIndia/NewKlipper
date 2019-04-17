using System;
using System.Collections.Generic;
using System.Text;
using DomainModel;
using UseCaseBoundary.DTO;

namespace UseCaseBoundary.Email
{
    public interface IEmailService
    {
        void SendMailForAddNewLeave(Leave takenLeave);
    }
}
