using Grpc.Net.Client;
using GrpcService;
using System;
using System.IO;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

class Program
{
    static async Task Main(string[] args)
    {

        var clientCertificate = new X509Certificate2("D:/Sertificate/certificate.pfx", "password");

        var handler = new HttpClientHandler();
        handler.ClientCertificates.Add(clientCertificate);
        handler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;


        var channel = GrpcChannel.ForAddress("https://localhost:8083", new GrpcChannelOptions { HttpHandler = handler });
        var client = new Greeter.GreeterClient(channel);

        int a;
        Console.Write("Enter a number: ");
        a = Convert.ToInt32(Console.ReadLine());
        if (a == 1)
        {
            string filePath = "D:/test.txt";
            byte[] fileBytes = File.ReadAllBytes(filePath);

            var reply = await client.FileAsync(new FileRequest
            {
                Data = Google.Protobuf.ByteString.CopyFrom(fileBytes),
                FileName = "test.txt",
                ChunkIndex = 1
            });
            Console.WriteLine(reply.Message);
            Console.WriteLine(string.Join(" ", fileBytes));
        }
        else
        {
            var reply = await client.TestAsync(new TestRequest
            {
                Name = "GreeterClient"
            });
            
            Console.WriteLine(reply.Message);
        }

        Console.ReadLine();
    }
}