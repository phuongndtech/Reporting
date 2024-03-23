using Reporting.Application;
using Reporting.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

var configurationBuilder = new ConfigurationBuilder();

var environment = builder.Environment.EnvironmentName;

IConfiguration configuration = configurationBuilder
    .AddJsonFile($"appsettings.{environment}.json")
    .Build();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(configuration);

builder.Services.AddControllers();

var app = builder.Build();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
