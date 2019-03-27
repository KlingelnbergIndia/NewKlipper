
namespace UseCaseBoundary
{
    public interface IAuthMongoDBRepository 
    {
        bool ChangePassword(int id, string password);
        int UserIdByUserName(string userName);
    }
}
