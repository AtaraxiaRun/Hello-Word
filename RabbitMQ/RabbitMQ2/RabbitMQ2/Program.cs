using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ2;

public class Program
{
    public static async Task Main(string[] args)
    {
        var p = Directory.GetCurrentDirectory();
        var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
        var configurationBuilder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddJsonFile($"appsettings.{environmentName}.json", true, reloadOnChange: true)
            .AddEnvironmentVariables();

        var configuration = configurationBuilder.Build();
        ApplicationConfig.SetConfiguration(configuration);
        var host = CreateHostBuilder(args).Build();
        await host.RunAsync();
    }

    /// <summary>
    ///  创建 HostBuilder 并配置服务
    /// </summary>
    /// <param name="args"></param>
    /// <returns></returns>
    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) =>
            {
                ConfigurationService(services);
                PostInitialize(services);
            });

    /// <summary>
    /// 服务注入
    /// </summary>
    /// <param name="services"></param>
    private static void ConfigurationService(IServiceCollection services)
    {
        services.AddSingleton<IProductService, ProductService>();
    }
    /// <summary>
    /// 初始化MQ消费服务
    /// </summary>
    /// <param name="services"></param>
    private static void PostInitialize(IServiceCollection services)
    {
        // 添加消费者服务
        services.AddHostedService<SyncProductWork>();
        services.AddHostedService<SyncProductWork2>();
    }
}

