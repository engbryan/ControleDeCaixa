using Amazon.Runtime;

namespace EntryControl.Contracts.Modules.Authentication.Services
{
    public interface IAWSCredentialsService
    {
        AWSCredentials AWSCredentials { get; }
        string AccessKeyID { get; }
        string SecretAccessKey { get; }

    }
}
