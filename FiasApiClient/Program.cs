using FiasApiClient.Services;
using FiasApiClient.Models;

var builder = WebApplication.CreateBuilder(args);

// Добавляем контроллеры
builder.Services.AddControllers();

// Регистрируем HttpClient для работы с Dadata
builder.Services.AddSingleton<DadataFiasClient>(sp =>
{
    var apiKey = builder.Configuration["DadataApiKey"]
        ?? Environment.GetEnvironmentVariable("DADATA_API_KEY")
        ?? "447b1e46c94767b89d8c19857e06143370c1b62b";

    return new DadataFiasClient(apiKey);
});

// Добавляем Swagger для тестирования
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

// Запускаем на всех интерфейсах
app.Urls.Add("http://0.0.0.0:8080");

Console.WriteLine("🚀 FIAS API Service запущен на http://localhost:8080");
Console.WriteLine("📚 Swagger: http://localhost:8080/swagger");

app.Run();