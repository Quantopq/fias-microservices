using Microsoft.EntityFrameworkCore;
using ClientApiService.Data;
using ClientApiService.Services;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// контроллеры
builder.Services.AddControllers();

//  DbContext для MSSQL
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? Environment.GetEnvironmentVariable("CONNECTION_STRING")
    ?? "Server=sqlserver;Database=FiasDb;User=sa;Password=YourPassword123!;TrustServerCertificate=True;";

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

// HttpClientFactory
builder.Services.AddHttpClient();

// сервисы
builder.Services.AddScoped<IClientService, ClientService>();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Swagger middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Urls.Add("http://0.0.0.0:8081");

Console.WriteLine("🚀 Client API Service запущен на http://localhost:8081");
Console.WriteLine("📚 Swagger: http://localhost:8081/swagger");

app.Run();