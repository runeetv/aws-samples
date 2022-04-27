using Amazon.Lambda.Core;
using NLog;
using NLog.Targets;
using NLog.Config;

using NLog.AWS.Logger;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace NLog_Sample;

public class Function
{

    /// <summary>
    /// A simple function that takes a string and does a ToUpper
    /// </summary>
    /// <param name="input"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public string FunctionHandler(string input, ILambdaContext context)
    {
        ConfigureNLog();

        ILogger logger = LogManager.GetCurrentClassLogger();
        logger.Info("Check the AWS Console CloudWatch Logs console in us-east-1 at " + DateTime.Now.ToString());
        logger.Info("to see messages in the log streams for the");
        logger.Info("log group NLog.ProgrammaticConfigurationExample");

        return input.ToUpper();
    }

    static void ConfigureNLog()
    {

        var config = new LoggingConfiguration();

        var consoleTarget = new ColoredConsoleTarget();
        config.AddTarget("console", consoleTarget);

        var awsTarget = new AWSTarget()
        {
            LogGroup = "NLog.ProgrammaticConfigurationExample",
            Region = "us-east-1"
        };
        config.AddTarget("aws", awsTarget);

        config.LoggingRules.Add(new LoggingRule("*", NLog.LogLevel.Debug, consoleTarget));
        config.LoggingRules.Add(new LoggingRule("*", NLog.LogLevel.Debug, awsTarget));

        LogManager.Configuration = config;
    }
}
