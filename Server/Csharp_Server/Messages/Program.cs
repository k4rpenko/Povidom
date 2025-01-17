using NoSQL;
using RedisDAL.User;
using RedisDAL;
using Messages.Hash;
using Messages.Hubs;
using Messages.Interface.Hash;
using Microsoft.EntityFrameworkCore;
using PGAdminDAL;
using Messages.Protection;

var builder = WebApplication.CreateBuilder(args);

// Підключення до бази даних PostgreSQL
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetSection("Npgsql:ConnectionString").Value));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddSignalR();
builder.Services.AddSignalR().AddJsonProtocol(options => { });

builder.Services.AddSingleton<AppMongoContext>();
builder.Services.AddSingleton<RedisConfigure>();
builder.Services.AddSingleton<UsersConnectMessage>();
builder.Services.AddHostedService<CharHubBackgroundService>();
builder.Services.AddScoped<IJwt, JWT>();
builder.Services.AddScoped<IHASH, HASH>();
builder.Services.AddScoped<IRSAHash, RSAHash>();


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder => builder.WithOrigins("http://localhost:4200")
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials());
});


var app = builder.Build();

app.UseMiddleware<RateLimitingMiddleware>();

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
            "script-src 'self' http://localhost:4200 https://localhost:8080 https://localhost:8081 'unsafe-inline' 'unsafe-eval'; " +
            "connect-src 'self'; " +
            "img-src 'self'; " +
            "style-src 'self' 'unsafe-inline';");

        return Task.CompletedTask;
    });

    await next();
});

app.UseCors("AllowSpecificOrigin");
app.MapHub<ChatHub>("/message");
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
await app.RunAsync();
