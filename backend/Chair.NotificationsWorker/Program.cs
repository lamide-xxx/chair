using Chair.NotificationsWorker.Services;
using DotNetEnv;
using Serilog;

Env.Load();
var logger = new LoggerConfiguration()
    .WriteTo.Console()
    .Enrich.FromLogContext()
    .CreateLogger();
var builder = Host.CreateApplicationBuilder(args);

// replace default providers with Serilog provider
builder.Logging.ClearProviders();
builder.Logging.AddSerilog(logger);

builder.Services.AddHostedService<SqsListener>();

var host = builder.Build();

try
{
    logger.Information("Starting Chair Notifications Worker...");
    await host.RunAsync();
}
catch (Exception ex)
{
    logger.Fatal(ex, "Chair Notifications Worker terminated unexpectedly");
}
finally
{
    await Log.CloseAndFlushAsync();
}
