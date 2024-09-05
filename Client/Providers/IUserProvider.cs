namespace EntryControl.Providers
{
    public interface IUserProvider
    {
        string GetMachineUser();
        string GetMachineDomainName();
        string GetApplicationUsername();
        string GetMachineUserDomainName();
    }
}