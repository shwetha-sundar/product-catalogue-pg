using Azure.Identity;
using CategoryMapper.FunctionApp.Data;
using CategoryMapper.FunctionApp.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureAppConfiguration((context, configBuilder) =>
    {
        configBuilder
            .AddJsonFile("local.settings.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables();
    })
    .ConfigureLogging(logging =>
    {
        logging.ClearProviders();
        logging.AddConsole();
    })
    .ConfigureServices((context, services) =>
    {
        var isLocal = Environment.GetEnvironmentVariable("AZURE_FUNCTIONS_ENVIRONMENT") == "Development";
        var connectionString = context.Configuration.GetConnectionString("ConnectionString");

        if (isLocal)
        {
            Console.WriteLine($"Connection String ... ${connectionString}");
            services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(connectionString)
                    .EnableSensitiveDataLogging()
                    .LogTo(Console.WriteLine));
        }
        else
        {
            try
            {
                Console.WriteLine("Configuring Npgsql with Azure AD authentication...");
                var tokenCredential = new DefaultAzureCredential();
                var accessToken = tokenCredential.GetToken(
                    new Azure.Core.TokenRequestContext(new[] { "https://ossrdbms-aad.database.windows.net/.default" })
                );

                Console.WriteLine($"Connection String ... ${connectionString}");
                var connectionStringWithToken = connectionString + $";Password={accessToken.Token}";

                Console.WriteLine($"Connection String ... ${connectionStringWithToken}");
                services.AddDbContext<AppDbContext>((provider, options) =>
                {
                    options.UseNpgsql(connectionStringWithToken)
                           .EnableSensitiveDataLogging()
                           .LogTo(Console.WriteLine);
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error configuring Npgsql: {ex.Message}");
            }
        }

        services.AddScoped<IAttributeService, AttributeService>();
        services.AddScoped<ICategoryService, CategoryService>();
    })
    .Build();

host.Run();