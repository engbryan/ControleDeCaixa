using Amazon.CognitoIdentityProvider;
using Amazon.Extensions.CognitoAuthentication;
using EntryControl.Core.Services;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;

public class CognitoAuthService : IAuthService
{
    private readonly IAmazonCognitoIdentityProvider _cognitoClient;
    private readonly CognitoUserPool _userPool;
    private readonly string _clientId;
    private readonly string _userPoolId;

    public bool IsAuthenticating { get; private set; }
    public bool IsAuthenticated { get; private set; }
    public CognitoUser User { get; private set; }

    public CognitoAuthService(IAmazonCognitoIdentityProvider cognitoClient, IConfiguration configuration)
    {
        _clientId = configuration["CognitoOptions:ClientId"] ?? throw new ArgumentNullException("Cognito ClientId is missing in configuration.");
        _userPoolId = configuration["CognitoOptions:UserPoolId"] ?? throw new ArgumentNullException("Cognito UserPoolId is missing in configuration.");

        _cognitoClient = cognitoClient;

        _userPool = new CognitoUserPool(_userPoolId, _clientId, _cognitoClient);
    }

    public async Task<string> LoginAsync(string username, string password)
    {
        User = new CognitoUser(username, _clientId, _userPool, _cognitoClient);

        var authRequest = new InitiateSrpAuthRequest()
        {
            Password = password
        };

        try
        {
            IsAuthenticating = true;

            var authResponse = await User.StartWithSrpAuthAsync(authRequest);

            IsAuthenticated = true;

            return authResponse.AuthenticationResult.IdToken;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erro no login: {ex.Message}");
            throw;
        }
        finally
        {
            IsAuthenticating = false;
        }
    }

    public void Logout()
    {
        if (User != null)
        {
            User.SignOut();
            IsAuthenticated = false;
        }
    }

    //public bool IsAuthenticated()
    //{
    //    return !string.IsNullOrWhiteSpace(User?.SessionTokens?.IdToken);
    //}

    public string GetToken()
    {
        return User?.SessionTokens?.IdToken;
    }

    public string GetUserName()
    {
        return User?.Username;
    }
}
