using Vehicles.Api.Interfaces;
using Vehicles.Api.Middleware;
using Vehicles.Api.Models;
using Vehicles.Api.Repositories;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Dependency Injection
builder.Services.AddTransient<IVehicleRepository, VehicleRepository>();

// Configuration
builder.Services.Configure<PathConfiguration>(builder.Configuration.GetSection("PathConfiguration"));

// Configure logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

var app = builder.Build();
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
