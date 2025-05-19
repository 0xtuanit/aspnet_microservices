namespace Basket.API.Extensions
{
    public static class HostExtensions
    {
        internal static void AddAppConfigurations(this ConfigurationManager config, IWebHostEnvironment env)
        {
            config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();
        }
    }
}