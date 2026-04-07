using Chats.Hubs;
using Hash;
using Hash.Interface;
using Microsoft.EntityFrameworkCore;
using MongoDB;
using PGAdminDAL;
using RedisDAL;
using System.Net;
using SessionService;
using Logger;
using RedisDAL.User;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(options =>
{
    options.Listen(IPAddress.Any, 8087, listenOptions =>
    {
        listenOptions.UseHttps("certificate.pfx", "password");
    });
});


builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetSection("Npgsql:ConnectionString").Value));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton(typeof(IAppLogger<>), typeof(AppLogger<>));

builder.Services.AddSignalR();
builder.Services.AddSignalR().AddJsonProtocol(options => { });

builder.Services.AddScoped<ISessionService, SessionService.SessionServiceImpl>();
builder.Services.AddSingleton<AppMongoContext>();
builder.Services.AddSingleton<RedisConfigure>();
builder.Services.AddScoped<RUsersConnectMessage>(sp =>
{
    var redisConfigure = sp.GetRequiredService<RedisConfigure>();
    return new RUsersConnectMessage(redisConfigure);
});
builder.Services.AddSingleton<RUsersConnectMessage>();
builder.Services.AddHostedService<CharHubBackgroundService>();
builder.Services.AddScoped<IHASH256, HASH256>();
builder.Services.AddScoped<IRSAHash, RSAHash>();


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder => builder.WithOrigins("https://localhost:4200")
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials());
});


var app = builder.Build();
app.UseCors("AllowSpecificOrigin");

app.Use(async (context, next) =>
{
    if (context.Request.Path.Value.Contains("@"))
    {
        context.Response.StatusCode = 403;
        await context.Response.WriteAsync("Access Denied");
        return;
    }

    context.Response.OnStarting(() =>
    {
        context.Response.Headers.Add("X-Frame-Options", "DENY");

        context.Response.Headers.Add("Content-Security-Policy",
            "default-src 'self'; " +
            "script-src 'self' http://localhost:4200 https://localhost:8086 https://localhost:8087 'unsafe-inline' 'unsafe-eval'; " +
            "connect-src 'self' http://localhost:4200 https://localhost:8087 wss://localhost:8087 ws://localhost:8087; " +
            "img-src 'self' data:; " +
            "style-src 'self' 'unsafe-inline';");

        return Task.CompletedTask;
    });

    await next();
});

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapHub<ChatHub>("/message");
app.MapControllers();
await app.RunAsync();