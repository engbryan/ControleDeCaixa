namespace EntryControl.POS.Core.Interfaces.Services
{
    public interface IUserService
    {
        string GetMachineUser();
        string GetMachineDomainName();
        string GetApplicationUsername();
        string GetMachineUserDomainName();
    }
}