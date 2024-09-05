using Amazon.Runtime;

namespace EntryControl.Core.Services
{
    public interface IAWSCredentialsService
    {
        AWSCredentials AWSCredentials { get; }
        string AccessKeyID { get; }
        string SecretAccessKey { get; }

    }
}
