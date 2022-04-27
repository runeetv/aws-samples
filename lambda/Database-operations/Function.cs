using Amazon;
using Amazon.Lambda.Core;
using Amazon.SecretsManager;
using Amazon.SecretsManager.Model;
using Newtonsoft.Json;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace Database_operations;

public class Function
{

    public async Task FunctionHandler(string input, ILambdaContext context)
    {
        Console.WriteLine("In function handler");

        Console.WriteLine("Getting Secret");
        string secretName = "RDS-SQL-Instance";
        string region = "us-east-1";
        string secret = "";

        MemoryStream memoryStream = new MemoryStream();

        IAmazonSecretsManager client = new AmazonSecretsManagerClient(RegionEndpoint.GetBySystemName(region));

        GetSecretValueRequest request = new GetSecretValueRequest();
        request.SecretId = secretName;
        request.VersionStage = "AWSCURRENT";

        GetSecretValueResponse response = null;

        Console.WriteLine("Calling GetSecretValueAsync");
        try
        {
            response = await client.GetSecretValueAsync(request);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Hit exception while retrieving secrets");
            Console.WriteLine(e.Message);
        }

        // Decrypts secret using the associated KMS key.
        // Depending on whether the secret is a string or binary, one of these fields will be populated.
        if (response.SecretString != null)
        {
            secret = response.SecretString;
            Console.WriteLine($"Retrieved SecretString is {secret }");
            // Sample Ouput
            //{
            //    "username": "username",
            //    "password": "Password",
            //    "engine": "sqlserver",
            //    "host": "db-instance-id.us-east-1.rds.amazonaws.com",
            //    "port": 1433,
            //    "dbInstanceIdentifier": "db-instance"
            //}
            dynamic connProperties = JsonConvert.DeserializeObject(secret);
            Console.WriteLine($"Servername : {connProperties.host }");
            Console.WriteLine($"username : {connProperties.username }");            

        }
        else
        {
            memoryStream = response.SecretBinary;
            StreamReader reader = new StreamReader(memoryStream);
            string decodedBinarySecret = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(reader.ReadToEnd()));
            Console.WriteLine($"Retrieved decodedBinarySecret is {decodedBinarySecret }");
        }

        return;
    }


}
