using System.Threading.Tasks;

namespace EntryControl.Core.Services
{
    public interface IAuthService
    {
        bool IsAuthenticating { get; }
        bool IsAuthenticated { get; }
        Task<string> LoginAsync(string username, string password);
        void Logout();
        //bool IsAuthenticated();
        string GetToken();
        string GetUserName();
    }
}
