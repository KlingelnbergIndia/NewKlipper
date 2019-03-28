using DataAccess;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataAccess.EntityModel.Authentication;
using UseCaseBoundary;
using UseCaseBoundary.DTO;

namespace RepositoryImplementation
{
    public class AuthMongoDBRepository:IAuthMongoDBRepository
    {
        private readonly AuthDBContext _authDBContext = null;

        public AuthMongoDBRepository()
        {
            _authDBContext = AuthDBContext.Instance;
        }

        public void ChangePassword(string username, string password)
        {
            _authDBContext.Users.UpdateOneAsync(
                x=>x.UserName.ToLower() == username.ToLower(), 
                Builders<UsersEntityModel>
                    .Update
                    .Set(a => a.PasswordHash,password));

        }

    }
}
