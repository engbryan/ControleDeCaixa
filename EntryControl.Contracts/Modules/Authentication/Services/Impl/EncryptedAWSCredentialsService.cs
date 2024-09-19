using Amazon.Runtime;
using EntryControl.Contracts.Modules.Authentication.Models;
using EntryControl.Contracts.Modules.Authentication.Services;


public class EncryptedAWSCredentialsService : IAWSCredentialsService
{
    public AWSCredentials AWSCredentials { get; set; }
    public string AccessKeyID { get; set; }
    public string SecretAccessKey { get; set; }

    public EncryptedAWSCredentialsService(CredentialsOptions credentials)
    {
        AccessKeyID = credentials.ID.Decrypt();
        SecretAccessKey = credentials.Secret.Decrypt();

        AWSCredentials = new BasicAWSCredentials(AccessKeyID, SecretAccessKey);
    }
}
