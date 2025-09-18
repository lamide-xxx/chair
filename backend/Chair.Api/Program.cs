using Chair.Domain.Repositories;
using Chair.Infrastructure.Persistence;
using Chair.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

//1. Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

//2. Register application services.
builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddSingleton<IUserRepository, UserRepository>();
builder.Services.AddSingleton<IStylistRepository, StylistRepository>();
builder.Services.AddScoped<IServiceRepository, ServiceRepository>();
builder.Services.AddSingleton<IAppointmentRepository, AppointmentRepository>();

//3. Build the app.
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers(); // Register controller routes automatically

app.MapGet("/health", () => Results.Ok(new{ status = "Healthy", service = "Chair.Api" }))
   .WithName("HealthCheck")
   .WithOpenApi();


app.Run();


