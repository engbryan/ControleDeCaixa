using EntryControl.Core.Services;
using System;

namespace EntryControl.Providers.Impl
{
    public class UserProvider : IUserProvider
    {
        public IAuthService _authService { get; }

        public UserProvider(IAuthService authService)
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