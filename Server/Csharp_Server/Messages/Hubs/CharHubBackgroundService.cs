using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using MongoDB.Driver;
using NoSQL;
using PGAdminDAL;
using RedisDAL.User;
using Messages.Hubs;
using Messages.Models.MessageChat;

public class CharHubBackgroundService : IHostedService
{
    private readonly IHubContext<ChatHub> _hubContext;
    private readonly IMongoCollection<ChatModelMongoDB> _customers;
    private readonly IServiceProvider _serviceProvider;
    private UsersConnectMessage _userConnect;
    private IServiceScope scope;
    private AppDbContext context;

    public CharHubBackgroundService(
        IHubContext<ChatHub> hubContext,
        IServiceProvider serviceProvider,
        UsersConnectMessage userConnect,
        AppMongoContext customers,
        IConfiguration configuration)
    {
        _hubContext = hubContext;
        _serviceProvider = serviceProvider;
        _userConnect = userConnect;
        _customers = customers.Database?.GetCollection<ChatModelMongoDB>(configuration.GetSection("MongoDB:MongoDbDatabaseChat").Value);
        scope = _serviceProvider.CreateScope();
        context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        _userConnect.UserRemoved += async (userId) => await HandleUserRemoval(userId);
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    public async Task HandleUserRemoval(string userId)
    {
        try
        {
            
            var user = await context.User.FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                Console.WriteLine("User not found");
                return;
            }
            user.IsOnline = false;
            await context.SaveChangesAsync();

            foreach (var chat in user.ChatsID)
            {
                var objectId = ObjectId.Parse(chat.ToString());
                var filter = Builders<ChatModelMongoDB>.Filter.Eq(chat => chat.Id, objectId);
                var chatModel = await _customers.Find(filter).FirstOrDefaultAsync();

                if (chatModel == null)
                {
                    Console.WriteLine("Chat model not found");
                    continue;
                }

                foreach (var userGET in chatModel.UsersID)
                {
                    if (userGET != userId)
                    {
                        var userConnection = await _userConnect.GetUserConnectionId(userGET);
                        if (userConnection != null)
                        {
                            Console.WriteLine("\n\nuserId: " + userId + "\nStatus: " + false);
                            await _hubContext.Clients.Client(userConnection).SendAsync("StatusUser", userId, false);
                        }
                    }
                }
            }
            
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception occurred: {ex.Message}");
            throw ex;
        }
    }
}
