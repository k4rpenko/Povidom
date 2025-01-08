using Server.Protection;
using PGAdminDAL;
using NoSQL;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RedisDAL;
using System;
using Server.Hubs;
using RedisDAL.User;
using Server.Sending;
using Server.Interface.Sending;
using Server.Interface.Hash;
using Server.Controllers;
using Server.Hash;

var builder = WebApplication.CreateBuilder(args);

// Підключення до бази даних PostgreSQL
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetSection("Npgsql:ConnectionString").Value));



builder.Services.AddSingleton<AppMongoContext>();
builder.Services.AddSingleton<RedisConfigure>();
builder.Services.AddSingleton<UsersConnectMessage>();
builder.Services.AddHostedService<CharHubBackgroundService>();
builder.Services.AddScoped<IEmailSeding, EmailSeding>();
builder.Services.AddScoped<IJwt, JWT>();
builder.Services.AddScoped<IHASH, HASH>();
builder.Services.AddScoped<IRSAHash, RSAHash>();



// Налаштування CORS (Cross-Origin Resource Sharing)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder => builder.WithOrigins("http://localhost:4200")  // Дозволено лише для цього домену
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials());
});

// Налаштування Identity для автентифікації та авторизації
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddDefaultTokenProviders()
    .AddDefaultUI()
    .AddEntityFrameworkStores<AppDbContext>();

// Налаштування SignalR для чатів у реальному часі
builder.Services.AddSignalR();
builder.Services.AddSignalR().AddJsonProtocol(options => { });

// Налаштування параметрів пароля та користувача для Identity
builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 1;

    options.User.AllowedUserNameCharacters =
    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.User.RequireUniqueEmail = false;
});

// Налаштування сесій
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SameSite = SameSiteMode.None;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});

// Налаштування API контролерів
builder.Services.AddControllers();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Додавання Middleware для безпеки
app.UseMiddleware<IpLoggingMiddleware>();
app.UseMiddleware<RateLimitingMiddleware>();

// Створення ролей, якщо вони ще не існують
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    var roles = new[] { "Admin", "User", "Moderator" };
    foreach (var roleName in roles)
    {
        if (!await roleManager.RoleExistsAsync(roleName))
        {
            var role = new IdentityRole(roleName);
            await roleManager.CreateAsync(role);
        }
    }

    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    try
    {
        await dbContext.Database.OpenConnectionAsync();
        await dbContext.Database.CloseConnectionAsync();
        Console.WriteLine("Ok");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"False: {ex.Message}");
    }
}


app.Use(async (context, next) =>
{
    // Перевірка, чи містить шлях @fs
    if (context.Request.Path.Value.Contains("@"))
    {
        context.Response.StatusCode = 403;  // Заборонити доступ
        await context.Response.WriteAsync("Access Denied");
        return;
    }

    // Додавання заголовків безпеки
    context.Response.OnStarting(() =>
    {
        // Додавання заголовка X-Frame-Options для захисту від clickjacking
        context.Response.Headers.Add("X-Frame-Options", "DENY");

        // Оновлена політика Content-Security-Policy
        context.Response.Headers.Add("Content-Security-Policy",
            "default-src 'self'; " +
            "script-src 'self' http://localhost:4200 https://localhost:8080 https://localhost:8081 'unsafe-inline' 'unsafe-eval'; " +
            "connect-src 'self'; " +
            "img-src 'self'; " +
            "style-src 'self' 'unsafe-inline';");

        return Task.CompletedTask;
    });

    // Продовжити обробку запиту
    await next();
});


// Додавання Swagger для API в режимі розробки
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Реєстрація SignalR Hub для чату в реальному часі
app.MapHub<ChatHub>("/message");
app.UseCors("AllowSpecificOrigin");
app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

// Реєстрація контролерів
app.MapControllers();

await app.RunAsync();
