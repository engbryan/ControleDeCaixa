namespace EntryControl.POS.Core.Interfaces.Services
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
