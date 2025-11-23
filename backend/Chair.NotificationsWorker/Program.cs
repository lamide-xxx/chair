using Chair.NotificationsWorker.Services;
using DotNetEnv;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
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
builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource.AddService("Chair.NotificationsWorker"))
    .WithTracing(tracing => tracing
        .AddSource("Chair.NotificationsWorker")
        .AddHttpClientInstrumentation()
        .AddOtlpExporter()
    )
    .WithMetrics(metrics => metrics
        .AddMeter("Chair.NotificationsWorker")
        .AddRuntimeInstrumentation()
        .AddOtlpExporter()
    );

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
