using Grpc.Core;
using GrpcService;

namespace MediaServer.Services
{
    public class GreeterService : Greeter.GreeterBase
    {
        public override Task<FileReply> File(FileRequest request, ServerCallContext context)
        {
            return Task.FromResult(new FileReply
            {
                Success = true,
                Message = "File: " + request.FileName
            });
        }
    }
}
