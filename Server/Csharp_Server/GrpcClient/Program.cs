using Grpc.Net.Client;
using GrpcService;

namespace GrpcClient
{
    class Program
    {
        static async Task Main(string[] args)
        {
            using var channel = GrpcChannel.ForAddress("https://localhost:8083");
            var client = new Greeter.GreeterClient(channel);
            string filePath = "D:/test.txt";
            byte[] fileBytes = File.ReadAllBytes(filePath);
            var reply = await client.FileAsync(new FileRequest { Data = Google.Protobuf.ByteString.CopyFrom(fileBytes), FileName = "test.txt", ChunkIndex = 1 });
            Console.WriteLine(reply);
            Console.WriteLine(string.Join(" ", fileBytes));
            Console.ReadLine();
        }
    }
}