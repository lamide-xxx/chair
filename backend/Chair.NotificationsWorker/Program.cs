using Chair.NotificationsWorker;
using Chair.NotificationsWorker.Services;
using DotNetEnv;

Env.Load();
var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<SqsListener>();

var host = builder.Build();
host.Run();
