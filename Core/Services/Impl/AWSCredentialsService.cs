using Amazon;
using Amazon.Runtime;
using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;
using EntryControl.Core.Services;
using System;
using System.Collections.Generic;
using System.Text.Json;


public class AWSCredentialsService : IAWSCredentialsService
{
    private readonly AmazonSecretsManagerClient _secretManagerClient;

    public AWSCredentials AWSCredentials { get; set; }
    public string AccessKeyID { get; set; }
    public string SecretAccessKey { get; set; }

    public AWSCredentialsService(RegionEndpoint regionEndpoint)
    {
        _secretManagerClient = new AmazonSecretsManagerClient(regionEndpoint);

        GetSecret();
    }

    void GetSecret()
    {
        GetSecretValueResponse response;

        try
        {
            response = _secretManagerClient.GetSecretValueAsync(new() { SecretId = "AccessKey" }).Result;
        }
        catch (Exception e)
        {
            throw;
        }

        var secret = JsonSerializer.Deserialize<Dictionary<string, string>>(response.SecretString);

        AccessKeyID = secret["AccessKeyID"];
        SecretAccessKey = secret["SecretAccessKey"];

        AWSCredentials = new BasicAWSCredentials(AccessKeyID, SecretAccessKey);
    }
}
