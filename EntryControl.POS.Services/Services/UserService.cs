using EntryControl.POS.Core.Interfaces.Services;

namespace EntryControl.POS.Services.Services
{
    public class UserService : IUserService
    {
        public IAuthService _authService { get; }

        public UserService(IAuthService authService)
        {
            _authService = authService;
        }

        public string GetMachineUser()
        {
            return Environment.UserName;
        }

        public string GetMachineDomainName()
        {
            return Environment.UserDomainName;
        }

        public string GetMachineUserDomainName()
        {
            return $"{Environment.UserDomainName}\\{Environment.UserName}";
        }

        public string GetApplicationUsername()
        {
            return _authService.GetUserName();
        }
    }
}