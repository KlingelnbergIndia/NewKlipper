
namespace UseCaseBoundary
{
    public interface IAuthMongoDBRepository 
    {
        void ChangePassword(string username, string password);
    }
}
