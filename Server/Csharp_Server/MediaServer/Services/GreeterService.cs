using Grpc.Core;
using GrpcService;

public class GreeterService : Greeter.GreeterBase
{
    private readonly ILogger<GreeterService> _logger;

    public GreeterService(ILogger<GreeterService> logger)
    {
        _logger = logger;
    }

    public override Task<FileReply> File(FileRequest request, ServerCallContext context)
    {
        _logger.LogInformation("Received file upload request: {FileName}", request.FileName);
        return Task.FromResult(new FileReply
        {
            Success = true,
            Message = "File: " + request.FileName
        });
    }

    public override Task<TestResponse> Test(TestRequest request, ServerCallContext context)
    {
        // Логування для перевірки отриманого запиту
        Console.WriteLine("Received gRPC request for test with name: " + request.Name);

        // Створення та відправка відповіді
        var response = new TestResponse { Message = "Hello, " + request.Name };
        Console.WriteLine("Sending response: " + response.Message);
        return Task.FromResult(response);
    }

}