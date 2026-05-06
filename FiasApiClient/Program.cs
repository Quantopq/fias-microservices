using FiasApiClient.Services;
using Microsoft.Extensions.Http;

var builder = WebApplication.CreateBuilder(args);

// контроллеры
builder.Services.AddControllers();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// HttpClient для FIAS API
builder.Services.AddHttpClient<IFiasService, FiasService>(client =>
{
    client.BaseAddress = new Uri("https://suggestions.dadata.ru/suggestions/api/4_1/rs");
    client.Timeout = TimeSpan.FromSeconds(100);
});

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

app.Run();