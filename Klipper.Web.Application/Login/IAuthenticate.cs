namespace Klipper.Web.Application.Login
{
    public interface IAuthenticate
    {
        bool Login(string userName, string password);
        string ResponseMessage { get; }
        LoginResponse ResponseStatus { get; }
    }
}