using Server.Protection;
using PGAdminDAL;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RedisDAL;
using System;

var builder = WebApplication.CreateBuilder(args);

// Налаштування контексту бази даних
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetSection("Npgsql:ConnectionString").Value));

builder.Services.AddSingleton<RedisConfigure>();

// Налаштування CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder => builder.WithOrigins("http://localhost:4200")
                          .AllowAnyHeader()
                          .AllowAnyMethod());
});

// Налаштування Identity
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddDefaultTokenProviders()
    .AddDefaultUI()
    .AddEntityFrameworkStores<AppDbContext>();

builder.Services.Configure<IdentityOptions>(options =>
{
    // Налаштування паролів
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 1;


    // Налаштування користувачів
    options.User.AllowedUserNameCharacters =
    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.User.RequireUniqueEmail = false;


});


builder.Services.AddControllers();


builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseMiddleware<RateLimitingMiddleware>();



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

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowSpecificOrigin");
app.UseAuthorization();
app.UseAuthentication();
app.MapControllers();

app.Run();