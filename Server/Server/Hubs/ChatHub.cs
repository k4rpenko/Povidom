using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Core.Connections;
using Newtonsoft.Json.Linq;
using NoSQL;
using PGAdminDAL;
using RedisDAL;
using RedisDAL.User;
using Server.Controllers;
using Server.Models.MessageChat;
using Server.Protection;
using System.Linq;
using System.Net.WebSockets;
using System.Text;

namespace Server.Hubs
{
    public class ChatHub : Hub
    {
        private readonly IMongoCollection<ChatModelMongoDB> _customers;
        private readonly AppDbContext context;
        private readonly UsersConnectMessage _userConnect;

        public ChatHub(AppMongoContext _Mongo, IConfiguration _configuration, AppDbContext _context, RedisConfigure redisConfigure)
        {
            _userConnect = new UsersConnectMessage(redisConfigure);
            _customers = _Mongo.Database?.GetCollection<ChatModelMongoDB>(_configuration.GetSection("MongoDB:MongoDbDatabaseChat").Value);
            context = _context;
            //_userConnect.SubscribeToMessages();
        }

        /*private async void SendInactivityNotification(string userId) 
        {
            var user = await context.User.FirstOrDefaultAsync(u => u.Id == userId);
            foreach (var chat in user.ChatsID) {
                var objectId = ObjectId.Parse(chat.ToString());
                var filter = Builders<ChatModelMongoDB>.Filter.Eq(chat => chat.Id, objectId);
                var chatModel = await _customers.Find(filter).FirstOrDefaultAsync();
                foreach (var userGET in chatModel.UsersID)
                {
                    if(userGET != userId)
                    {
                        var userConnection = await _userConnect.GetUserConnection(userGET); 
                        var connectionId = userConnection.FirstOrDefault(entry => entry.Name == "connectionId").Value; 
                        await Clients.Client(connectionId).SendAsync("StatusUser", userId, false);
                    }
                }
            }
        }*/


        public async Task<bool> Connect(string token)
        {
            try
            {
                var id = new JWT().GetUserIdFromToken(token);
                var user = await context.User.FirstOrDefaultAsync(u => u.Id == id);
                if (user != null)
                {
                    _userConnect.UpdateUserConnection(id, Context.ConnectionId);
                    user.ConnectionId = Context.ConnectionId;
                    user.IsOnline = true;
                    await context.SaveChangesAsync();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending message: {ex.Message}");
                return false;
            }
        }
        
        public async Task<List<SendChats>> disconnection(string token)
        {
            try
            {
                var id = new JWT().GetUserIdFromToken(token);
                var user = await context.User.FirstOrDefaultAsync(u => u.Id == id);
                if (user != null)
                {
                    _userConnect.RemoveUser(id);
                    user.IsOnline = true;
                    await context.SaveChangesAsync();
                }
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending message: {ex.Message}");
                return null;
            }
        }

        public async Task<List<SendChats>> Update(string token)
        {
            try
            {
                var id = new JWT().GetUserIdFromToken(token);
                var user = await context.User.FirstOrDefaultAsync(u => u.Id == id);
                if (user != null)
                {
                    _userConnect.UpdateUserConnection(id, Context.ConnectionId);
                    user.IsOnline = true;
                    await context.SaveChangesAsync();
                }
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending message: {ex.Message}");
                return null;
            }
        }



        public async Task<SendChats> CreateChat(MessageModel _get)
        {
            if (_get == null || _get.AddUsersIdChat == null || _get.AddUsersIdChat.Count < 2)
            {
                throw new ArgumentException("Invalid chat model or user IDs.");
            }

            var id = new JWT().GetUserIdFromToken(_get.AddUsersIdChat[0]);

            var filter = Builders<ChatModelMongoDB>.Filter.And(
                Builders<ChatModelMongoDB>.Filter.Or(
                    Builders<ChatModelMongoDB>.Filter.Eq(chat => chat.UsersID[0], id),
                    Builders<ChatModelMongoDB>.Filter.Eq(chat => chat.UsersID[0], _get.AddUsersIdChat[1])
                ),
                Builders<ChatModelMongoDB>.Filter.Or(
                    Builders<ChatModelMongoDB>.Filter.Eq(chat => chat.UsersID[1], id),
                    Builders<ChatModelMongoDB>.Filter.Eq(chat => chat.UsersID[1], _get.AddUsersIdChat[1])
                )
            );

            var You = await context.User.FirstOrDefaultAsync(u => u.Id == id);
            var People = await context.User.FirstOrDefaultAsync(u => u.Id == _get.AddUsersIdChat[1]);

            if (You == null || People == null)
            {
                throw new Exception("One or both users not found.");
            }

            var PeopleInfo = new SendChats
            {
                LastMessage = new LastMessage
                {
                    UserId = People.Id,
                    Message = "",
                },
                Avatar = People.Avatar,
                NickName = People.FirstName
            };

            var existingChat = await _customers.Find(filter).FirstOrDefaultAsync();

            if (existingChat != null)
            {
                PeopleInfo.ChatId = existingChat.Id.ToString();
                return PeopleInfo;
            }

            _get.AddUsersIdChat[0] = id;
            var newChat = new ChatModelMongoDB
            {
                UsersID = _get.AddUsersIdChat,
                Timestamp = DateTime.UtcNow
            };

            var YouInfo = new SendChats
            {
                ChatId = newChat.UsersID.ToString(),
                LastMessage = new LastMessage
                {
                    UserId = You.Id,
                    Message = "",
                },
                Avatar = You.Avatar,
                NickName = You.FirstName
            };

            foreach (var userId in newChat.UsersID)
            {
                var user = await context.User.FirstOrDefaultAsync(u => u.Id == userId);
                await Clients.Client(user.ConnectionId).SendAsync("CreatChat", YouInfo);
            }

            await _customers.InsertOneAsync(newChat);
            PeopleInfo.ChatId = newChat.Id.ToString();

            You.ChatsID.Add(newChat.Id.ToString());
            People.ChatsID.Add(newChat.Id.ToString());

            await context.SaveChangesAsync();
            return PeopleInfo;
        }

        public async Task<List<SendChats>> GetChats(string token)
        {
            try
            {
                var id = new JWT().GetUserIdFromToken(token);
                var user = await context.User.FirstOrDefaultAsync(u => u.Id == id);
                if (user != null)
                {
                    if (user.ChatsID != null && user.ChatsID.Count > 0)
                    {
                        var ChatsData = new List<SendChats>();
                        foreach (var GetChatID in user.ChatsID)
                        {
                            var objectIds = user.ChatsID.Select(chatId => ObjectId.Parse(GetChatID.ToString())).ToList();
                            var filter = Builders<ChatModelMongoDB>.Filter.In(chat => chat.Id, objectIds);

                            var chatModels = await _customers.Find(filter).ToListAsync();


                            foreach (var chatModel in chatModels)
                            {
                                var Ac = await context.User
                                    .Where(u => chatModel.UsersID.Contains(u.Id) && u.Id != id)
                                    .ToListAsync();

                                foreach (var account in Ac)
                                {
                                    var lastMessage = chatModel.Chat.Count > 0 ? chatModel.Chat[^1] : null;

                                    var chatInfo = new SendChats
                                    {
                                        ChatId = chatModel.Id.ToString(),
                                        LastMessage = new LastMessage
                                        {
                                            UserId = lastMessage?.IdUser ?? "",
                                            Message = lastMessage?.Text ?? ""
                                        },
                                        Avatar = account.Avatar,
                                        NickName = account.UserName,
                                        CreatedAt = lastMessage?.CreatedAt,
                                        View = lastMessage?.View,
                                        CreatorId = lastMessage?.IdUser
                                    };

                                    ChatsData.Add(chatInfo);
                                }
                            }
                        }
                        return ChatsData;
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n\n\n\n\n Error sending message: {ex.Message}\n\n\n");
                return null;
            }

        }



        public async Task<int> SendMessage(MessageModel _get)
        {
            try
            {
                var id = new JWT().GetUserIdFromToken(_get.CreatorId);
                var objectId = ObjectId.Parse(_get.IdChat.ToString());
                var filter = Builders<ChatModelMongoDB>.Filter.Eq(chat => chat.Id, objectId);
                var chatModel = await _customers.Find(filter).FirstOrDefaultAsync();

                
                if (!chatModel.UsersID.Contains(id))
                {
                    Console.WriteLine("Chat ID not found in user IDs.");
                    return -1;
                }

                

                var lastMessageId = chatModel.Chat?.Count > 0
                    ? chatModel.Chat.OrderByDescending(m => m.CreatedAt).FirstOrDefault()?.Id ?? 0
                    : 0;

                var newMessage = new MessageModel
                {
                    IdChat = _get.IdChat,
                    message = new Message{
                        Id = lastMessageId + 1,
                        IdUser = id,
                        Text = _get.Text,
                        Img = string.IsNullOrEmpty(_get.Img?.ToString()) ? null : _get.Img.ToString(),
                        IdAnswer = string.IsNullOrEmpty(_get.IdAnswer?.ToString()) ? null : _get.Img.ToString(),
                        CreatedAt = DateTime.UtcNow,
                        View = false,
                        Send = true
                    }
                };

                var update = Builders<ChatModelMongoDB>.Update
                    .Push(chat => chat.Chat, newMessage.message)
                    .Set(chat => chat.Timestamp, DateTime.UtcNow);

                var updateResult = await _customers.UpdateOneAsync(filter, update);

                if (updateResult.MatchedCount == 0)
                {
                    return -1;
                }


                foreach (var userId in chatModel.UsersID)
                {
                    if(userId != id)
                    {
                        var user = await context.User.FirstOrDefaultAsync(u => u.Id == userId);
                        Console.WriteLine("\n\n\n " + user.ConnectionId);
                        await Clients.Client(user.ConnectionId).SendAsync("ReceiveMessage", newMessage);
                    }
                }

                return lastMessageId;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending message: {ex.Message}");
                return -1;
            }
        }

        public async Task<List<Message>> GetMessage(MessageModel _get)
        {
            try
            {
                var id = new JWT().GetUserIdFromToken(_get.CreatorId);
                var user = await context.User.FirstOrDefaultAsync(u => u.Id == id);
                if (user != null)
                {
                    var objectId = ObjectId.Parse(_get.IdChat.ToString());
                    var filter = Builders<ChatModelMongoDB>.Filter.Eq(chat => chat.Id, objectId);
                    var chatModel = await _customers.Find(filter).FirstOrDefaultAsync();
                    List<string> idMessage;

                    if (chatModel != null && chatModel.UsersID.Contains(id))
                    {
                        foreach (var message in chatModel.Chat)
                        {
                            if (message.IdUser != id && message.View == false)
                            {
                                message.View = true;
                            }
                        }
                        var update = Builders<ChatModelMongoDB>.Update.Set(chat => chat.Chat, chatModel.Chat);
                        await _customers.UpdateOneAsync(filter, update);

                        return chatModel.Chat;
                    }
                    else
                    {
                        Console.WriteLine("Chat ID not found in user IDs.");
                        return null;
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending message: {ex.Message}");
                return null;
            }
        }



        public async Task<string> GetId(string token)
        {
            try
            {
                var id = new JWT().GetUserIdFromToken(token);
                var user = await context.User.FirstOrDefaultAsync(u => u.Id == id);
                if (user != null)
                {
                    return user.Id;
                }
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending message: {ex.Message}");
                return null;
            }
        }

        public async Task<List<StatusModel>> GetStatus(string token)
        {
            var id = new JWT().GetUserIdFromToken(token);
            var user = await context.User.FirstOrDefaultAsync(u => u.Id == id);
            if (user != null)
            {
                List<StatusModel> status = new List<StatusModel>(); 
                foreach (var chatId in user.ChatsID)
                {
                    var objectId = ObjectId.Parse(chatId.ToString());
                    var filter = Builders<ChatModelMongoDB>.Filter.Eq(chat => chat.Id, objectId);
                    var chatModel = await _customers.Find(filter).FirstOrDefaultAsync();

                    if (chatModel != null) 
                    {
                        foreach (var userId in chatModel.UsersID)
                        {
                            if (userId != id) 
                            {
                                var Status = await context.User.FirstOrDefaultAsync(u => u.Id == userId);
                                if (Status != null)
                                {
                                    var statuss = new StatusModel
                                    {
                                        UserId = userId,
                                        IsOnline = Status.IsOnline
                                    };
                                    status.Add(statuss);
                                }
                            }
                        }
                    }
                }
                return status;
            }
            return null;
        }

        public async Task<bool> ViewMessage(MessageModel _get)
        {
            try
            {
                var objectId = ObjectId.Parse(_get.IdChat.ToString());
                var filter = Builders<ChatModelMongoDB>.Filter.Eq(chat => chat.Id, objectId);
                var chatModel = await _customers.Find(filter).FirstOrDefaultAsync();
                if (chatModel != null && chatModel.UsersID.Contains(_get.CreatorId))
                {
                    foreach (var message in chatModel.Chat)
                    {
                        if (message.IdUser == _get.CreatorId && message.View == false)
                        {
                            message.View = true;
                        }
                    }
                    var update = Builders<ChatModelMongoDB>.Update.Set(chat => chat.Chat, chatModel.Chat);
                    await _customers.UpdateOneAsync(filter, update);
                    foreach (var userId in chatModel.UsersID)
                    {
                        if (userId == _get.CreatorId)
                        {
                            var user = await context.User.FirstOrDefaultAsync(u => u.Id == userId);
                            await Clients.Client(user.ConnectionId).SendAsync("ViewMessage", _get.IdChat);
                        }
                    }
                    return true;
                }
                Console.WriteLine("Chat ID not found in user IDs.");
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}