using Grpc.Net.Client;
using TestService.Services;

namespace TestService
{
    public class Server
    {
        public static void Run(ILoggerFactory loggerFactory, string[] args)
        {
            var logger = loggerFactory.CreateLogger<Server>();

            logger.LogInformation("Server thread started");

            var builder = WebApplication.CreateBuilder(args);
            builder.WebHost.UseUrls("https://localhost:7042");

            // Additional configuration is required to successfully run gRPC on macOS.
            // For instructions on how to configure Kestrel and gRPC clients on macOS, visit https://go.microsoft.com/fwlink/?linkid=2099682

            // Add services to the container.
            builder.Services.AddGrpc();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            app.MapGrpcService<GreeterService>();
            app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

            try
            {
                app.Run();
            }
            catch (ThreadInterruptedException e)
            {
                // ignore
            }
        }
    }

    public class Client
    {
        public static void Run(ILoggerFactory loggerFactory)
        {
            var logger = loggerFactory.CreateLogger<Client>();

            logger.LogInformation("Client thread started");

            // The port number must match the port of the gRPC server.
            using var channel = GrpcChannel.ForAddress("https://localhost:7042");

            logger.LogInformation("Client channel created");

            var client = new Greeter.GreeterClient(channel);

            logger.LogInformation("Client created");

            var reply = client.SayHello(new HelloRequest { Message = "GreeterClient" });

            logger.LogInformation("Sending request");

            logger.LogInformation("Greeting: " + reply.Message);
        }

    }

    public class Program
    {
        public static void Main(string[] args)
        {
            var loggerFactory = LoggerFactory.Create(config =>
            {
                config.AddConsole();
                config.AddDebug();
                config.AddEventSourceLogger();
            });
            var logger = loggerFactory.CreateLogger<Program>();

            logger.LogInformation("Program started");

            var serverThread = new Thread(() => Server.Run(loggerFactory, args));
            serverThread.Start();

            Thread.Sleep(2 * 1000);

            var clientThread = new Thread(() => Client.Run(loggerFactory));
            clientThread.Start();

            Thread.Sleep(10 * 1000);
            clientThread.Interrupt();
            serverThread.Interrupt();
        }
    }
}
