
var builder = WebApplication.CreateBuilder(args);



// 🔑 Получение API ключей из переменных окружения
var deepSeekApiKey = builder.Configuration["DeepSeek:ApiKey"]
                     ?? Environment.GetEnvironmentVariable("DEEPSEEK_API_KEY");

// DeepSeek HTTP Client
builder.Services.AddHttpClient<DeepSeekLeadGenerator>(client =>
{
    var apiKey = builder.Configuration["DeepSeek:ApiKey"]
                 ?? Environment.GetEnvironmentVariable("DEEPSEEK_API_KEY");

    client.BaseAddress = new Uri("https://api.deepseek.com/v1");
    client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
});

// DB Context
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

// JWT
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
builder.Services.Configure<JwtSettings>(jwtSettings);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]!))
    };
});

builder.Services.AddAuthorization();
builder.Services.AddScoped<AuthService>();

// Controllers
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<ClientApiService.Interfaces.IClientService, ClientApiService.Services.ClientService>();



// DeepSeek HTTP Client
builder.Services.AddHttpClient<DeepSeekLeadGenerator>();

// Lead Generator
builder.Services.AddSingleton<ILeadGeneratorService, DeepSeekLeadGenerator>();

// Background Service
builder.Services.AddHostedService<LeadRandomizerBackgroundService>();

var app = builder.Build();

// Migrate DB 
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    // Вместо миграций — автоматическое создание таблиц:
    db.Database.EnsureCreated();

    // Seed ролей вручную (если их нет)
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var roles = new[] { "Admin", "Operator", "Viewer" };
    foreach (var roleName in roles)
    {
        if (!await roleManager.RoleExistsAsync(roleName))
        {
            await roleManager.CreateAsync(new IdentityRole(roleName));
        }
    }

    // Seed админа (если нет)
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
    if (await userManager.FindByEmailAsync("admin@fias.local") == null)
    {
        var admin = new ApplicationUser
        {
            UserName = "admin@fias.local",
            Email = "admin@fias.local",
            FullName = "Admin User"
        };
        await userManager.CreateAsync(admin, "Admin123!");
        await userManager.AddToRoleAsync(admin, "Admin");
    }
}


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();