using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using Amazon.Lambda.APIGatewayEvents;
using Amazon.Lambda.Core;
using Amazon.Lambda.RuntimeSupport;
using Amazon.Lambda.Serialization.SystemTextJson;
using System;
using System.Text.Json;
using System.Threading.Tasks;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace EntryControl.Lambdas
{
    public class Function
    {
        private static async Task Main(string[] args)
        {
            Func<AuthRequest, ILambdaContext, Task<APIGatewayProxyResponse>> handler = FunctionHandler;
            await LambdaBootstrapBuilder.Create(handler, new DefaultLambdaJsonSerializer())
                .Build()
                .RunAsync();
        }

        public static async Task<APIGatewayProxyResponse> FunctionHandler(AuthRequest authRequest, ILambdaContext context)
        {
            try
            {
                var cognitoClient = new AmazonCognitoIdentityProviderClient();
                var userPoolId = Environment.GetEnvironmentVariable("COGNITO_USER_POOL_ID") ?? throw new ArgumentNullException("COGNITO_USER_POOL_ID");
                var clientId = Environment.GetEnvironmentVariable("COGNITO_CLIENT_ID") ?? throw new ArgumentNullException("COGNITO_CLIENT_ID");

                var authResponse = await cognitoClient.InitiateAuthAsync(new InitiateAuthRequest
                {
                    AuthFlow = AuthFlowType.USER_PASSWORD_AUTH,
                    ClientId = clientId,
                    AuthParameters = new System.Collections.Generic.Dictionary<string, string>
                    {
                        { "USERNAME", authRequest.Username },
                        { "PASSWORD", authRequest.Password }
                    }
                });

                context.Logger.LogLine($"User {authRequest.Username} authenticated successfully.");

                return new APIGatewayProxyResponse
                {
                    StatusCode = 200,
                    Body = JsonSerializer.Serialize(new { authResponse.AuthenticationResult.IdToken })
                };
            }
            catch (Exception ex)
            {
                context.Logger.LogLine($"Error authenticating user {authRequest.Username}: {ex.Message}");
                throw new UnauthorizedAccessException("Authentication failed.");
            }
        }

        public class AuthRequest
        {
            public string Username { get; set; }
            public string Password { get; set; }
        }
    }

}
