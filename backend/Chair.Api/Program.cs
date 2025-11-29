using System.Threading.RateLimiting;
using Chair.Domain.Messaging;
using Chair.Domain.Repositories;
using Chair.Infrastructure.Messaging;
using Chair.Infrastructure.Persistence;
using Chair.Infrastructure.Repositories;
using DotNetEnv;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;

Env.Load();
var myAllowSpecificOrigins = "_myAllowSpecificOrigins";

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .Enrich.FromLogContext()
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

//1. Add services to the container.
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: myAllowSpecificOrigins,
        policy  =>
        {
            // policy.WithOrigins("http://localhost:3000") // Adjust the origin as needed;
            policy.AllowAnyOrigin()
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = 429;
    options.AddFixedWindowLimiter("global", limiterOptions =>
    {
        limiterOptions.PermitLimit = 200; // max 200 requests
        limiterOptions.Window = TimeSpan.FromSeconds(30); // per 30s window
        limiterOptions.QueueLimit = 20; // allow 20 extra queued
        limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
    });
});
builder.Services.AddControllers();
builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource.AddService("Chair.Api"))
    .WithTracing(tracing => tracing
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddEntityFrameworkCoreInstrumentation()
        .AddOtlpExporter()
    )
    .WithMetrics(metrics => metrics
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        .AddRuntimeInstrumentation()
        .AddOtlpExporter()
    );

//2. Register application services.
var connectionString = Environment.GetEnvironmentVariable("DATABASE_URL") 
                       ?? builder.Configuration.GetConnectionString("DefaultConnection");
var notificationsQueueUrl = Environment.GetEnvironmentVariable("SQS_QUEUE_URL") 
               ?? builder.Configuration.GetValue<string>("SqsQueueUrl")
               ?? throw new InvalidOperationException("Missing Notifications Queue URL");
builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IStylistRepository, StylistRepository>();
builder.Services.AddScoped<IServiceRepository, ServiceRepository>();
builder.Services.AddScoped<IAppointmentRepository, AppointmentRepository>();
builder.Services.AddSingleton<IMessagePublisher>(new SqsPublisher(notificationsQueueUrl));

//3. Build the app.
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseHsts(); // HTTP Strict Transport Security
app.UseXContentTypeOptions(); // Prevent MIME-type sniffing
app.UseReferrerPolicy(opt => opt.NoReferrer());
app.UseXXssProtection(opt => opt.EnabledWithBlockMode());
app.UseXfo(opt => opt.Deny()); // Frame options

app.UseCors(myAllowSpecificOrigins);
app.UseRateLimiter();
app.MapControllers().RequireRateLimiting("global"); // Register controller routes automatically

app.MapGet("/healthz", () =>
{
    return Results.Ok(new
    {
        status = "Healthy",
        service = "Chair.Api",
        time = DateTime.UtcNow
    });
}).WithName("HealthCheck");

app.MapDelete("/api/cleanup-test-user", async (AppDbContext db) =>
    {
        var testUserId = new Guid("2cd692eb-d4b8-4c1c-8026-c3677754bf30");
        var testAppointments = db.Appointments.Where(a => a.UserId == testUserId);

        var count = await testAppointments.CountAsync();
        if (count == 0)
        {
            return Results.Ok(new { message = "No test data found." });
        }

        db.Appointments.RemoveRange(testAppointments);
        await db.SaveChangesAsync();

        return Results.Ok(new { message = $"Cleaned up {count} test appointments." });
    })
    .WithName("CleanupTestUser")
    .WithOpenApi();


await app.RunAsync();


