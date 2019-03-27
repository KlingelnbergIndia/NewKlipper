using DataAccess;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataAccess.EntityModel.Authentication;
using UseCaseBoundary.DTO;

namespace RepositoryImplementation
{
    public class AuthMongoDBRepository
    {
        private readonly AuthDBContext _authDBContext = null;

        public AuthMongoDBRepository()
        {
            _authDBContext = AuthDBContext.Instance;
        }

        public bool ChangePassword(int id, string password)
        {
            _authDBContext.Users.UpdateOneAsync(
                x=>x.ID == id, 
                Builders<UsersEntityModel>
                    .Update
                    .Set(a => a.PasswordHash,password));

            return true;
        }

        public int UserIdByUserName(string userName)
        {
           var user = _authDBContext.Users.FindAsync(x=>x.UserName.ToLower() == userName);
           return user.Id;
        }

    }
}
