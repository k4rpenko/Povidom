using System.Net;
using System.Security.Cryptography.X509Certificates;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();


var certificate = new X509Certificate2("certificate.pfx", "password");
// Завантажуємо сертифікат і ключ з файлів
builder.WebHost.ConfigureKestrel(options =>
{
    options.Listen(IPAddress.Any, 8083, listenOptions =>
    {

        listenOptions.UseHttps(certificate);
    });
});


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddGrpc();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Закоментуйте або видаліть наступний рядок, якщо не хочете редирект на HTTPS
// app.UseHttpsRedirection();

app.UseAuthorization();
app.MapControllers();

// Map the gRPC service
app.MapGrpcService<GreeterService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client.");

await app.RunAsync();