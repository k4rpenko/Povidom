using AdminServer.Hash;
using AdminServer.Protection;
using AdminServer.Sending;
using NoSQL;
using RedisDAL.User;
using RedisDAL;
using SerAdminServerver.Hash;
using Microsoft.EntityFrameworkCore;
using PGAdminDAL;
using AdminServer.Interface.Sending;
using AdminServer.Interface.Hash;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetSection("Npgsql:ConnectionString").Value));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<AppMongoContext>();
builder.Services.AddSingleton<RedisConfigure>();
builder.Services.AddSingleton<UsersConnectMessage>();
builder.Services.AddScoped<IEmailSeding, EmailSeding>();
builder.Services.AddScoped<IJwt, JWT>();
builder.Services.AddScoped<IHASH, HASH>();
builder.Services.AddScoped<IRSAHash, RSAHash>();

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

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

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
await app.RunAsync();
