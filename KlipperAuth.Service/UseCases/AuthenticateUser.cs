using System;
using System.Collections.Generic;
using System.Text;

namespace KlipperAuth.Service.UseCases
{
    public class AuthenticateUser
    {
        private IUserRepository _userRepository;
        public AuthenticateUser(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }


    }
}
