using Chair.Domain.Messaging;
using Chair.Domain.Repositories;
using Chair.Infrastructure.Messaging;
using Chair.Infrastructure.Persistence;
using Chair.Infrastructure.Repositories;
using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using Serilog;

Env.Load();
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

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
    options.AddPolicy(name: MyAllowSpecificOrigins,
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
builder.Services.AddControllers();

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
app.UseCors(MyAllowSpecificOrigins);
app.MapControllers(); // Register controller routes automatically

app.MapGet("/health", () => Results.Ok(new{ status = "Healthy", service = "Chair.Api" }))
   .WithName("HealthCheck")
   .WithOpenApi();


app.Run();


