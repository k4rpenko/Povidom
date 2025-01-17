using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using MongoDB.Bson;
using MongoDB.Driver;
using NoSQL;
using PGAdminDAL;
using RedisDAL.User;
using Messages.Interface;
using Messages.Models.MessageChat;
using System.Net.WebSockets;
using Messages.Hash;

namespace Messages.Hubs
{
    public class ChatHub : Hub, IChatHub
    {
        private readonly IMongoCollection<ChatModelMongoDB> _customers;
        private readonly AppDbContext context;
        private UsersConnectMessage _userConnect;


        public ChatHub(AppMongoContext _Mongo, IConfiguration _configuration, AppDbContext _context, UsersConnectMessage UCM)
        {
            _customers = _Mongo.Database?.GetCollection<ChatModelMongoDB>(_configuration.GetSection("MongoDB:MongoDbDatabaseChat").Value);
            context = _context;
            _userConnect = UCM;
        }


        public async Task<bool> Connect(string token)
        {
            try
            {
                var id = new JWT().GetUserIdFromToken(token);
                var user = await context.User.FirstOrDefaultAsync(u => u.Id == id);
                if (user != null)
                {
                    await _userConnect.UpdateUserConnection(id, Context.ConnectionId);
                    user.IsOnline = true;
                    await context.SaveChangesAsync();
                    foreach (var chat in user.ChatsID)
                    {
                        var objectId = ObjectId.Parse(chat.ToString());
                        var filter = Builders<ChatModelMongoDB>.Filter.Eq(chat => chat.Id, objectId);
                        var chatModel = await _customers.Find(filter).FirstOrDefaultAsync();
                        foreach (var userGET in chatModel.UsersID)
                        {
                            if (userGET != id)
                            {
                                var userConnection = await _userConnect.GetUserConnectionId(userGET);
                                if (userConnection != null)
                                {
                                    await Clients.Client(userConnection).SendAsync("StatusUser", id, true);
                                }
                            }
                        }
                    }
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        
        public async Task<List<Chats>> disconnection(string token)
        {
            try
            {
                var id = new JWT().GetUserIdFromToken(token);
                var user = await context.User.FirstOrDefaultAsync(u => u.Id == id);
                if (user != null)
                {
                    await _userConnect.RemoveUser(id);
                    user.IsOnline = true;
                    await context.SaveChangesAsync();
                }
                return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task Update(string token)
        {
            try
            {
                var id = new JWT().GetUserIdFromToken(token);
                var user = await context.User.FirstOrDefaultAsync(u => u.Id == id);
                if (user != null)
                {
                    await _userConnect.UpdateUserConnection(id, Context.ConnectionId);
                    foreach (var chat in user.ChatsID)
                    {
                        var objectId = ObjectId.Parse(chat.ToString());
                        var filter = Builders<ChatModelMongoDB>.Filter.Eq(chat => chat.Id, objectId);
                        var chatModel = await _customers.Find(filter).FirstOrDefaultAsync();
                        foreach (var userGET in chatModel.UsersID)
                        {
                            if (userGET != id)
                            {
                                var userConnection = await _userConnect.GetUserConnectionId(userGET);
                                if (userConnection != null)
                                {
                                    await Clients.Client(userConnection).SendAsync("StatusUser", id, true);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }



        public async Task<Chats> CreateChat(Chats _get)
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

            var PeopleInfo = new Chats
            {
                Message = new Message 
                {
                    IdUser = People.Id,
                    Text = "",
                },
                User = new UserFind
                {
                    Avatar = People.Avatar,
                    UserName = People.FirstName
                }
            };

            var existingChat = await _customers.Find(filter).FirstOrDefaultAsync();

            if (existingChat != null)
            {
                PeopleInfo.Id = existingChat.Id.ToString();
                return PeopleInfo;
            }

            _get.AddUsersIdChat[0] = id;
            var newChat = new ChatModelMongoDB
            {
                UsersID = _get.AddUsersIdChat,
                Timestamp = DateTime.UtcNow
            };

            var YouInfo = new Chats
            {
                Id = newChat.UsersID.ToString(),
                Message = new Message
                {
                    IdUser = You.Id,
                    Text = "",
                },
                User = new UserFind
                {
                    Avatar = People.Avatar,
                    UserName = People.FirstName
                }
            };

            foreach (var userId in newChat.UsersID)
            {
                var ConnectId = await _userConnect.GetUserConnectionId(userId);
                await Clients.Client(ConnectId).SendAsync("CreatChat", YouInfo);
            }

            await _customers.InsertOneAsync(newChat);
            PeopleInfo.Id = newChat.Id.ToString();

            You.ChatsID.Add(newChat.Id.ToString());
            People.ChatsID.Add(newChat.Id.ToString());

            await context.SaveChangesAsync();
            return PeopleInfo;
        }

        public async Task<List<Chats>> GetChats(string token)
        {
            try
            {
                var id = new JWT().GetUserIdFromToken(token);
                var user = await context.User.FirstOrDefaultAsync(u => u.Id == id);
                if (user != null)
                {
                    if (user.ChatsID != null && user.ChatsID.Count > 0)
                    {
                        var ChatsData = new List<Chats>();
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

                                    var chatInfo = new Chats
                                    {
                                        Id = chatModel.Id.ToString(),
                                        Message = new Message
                                        {
                                            IdUser = lastMessage?.IdUser ?? "",
                                            Text = lastMessage?.Text ?? "",
                                            CreatedAt = lastMessage?.CreatedAt,
                                            View = lastMessage?.View,
                                        },
                                        User = new UserFind 
                                        { 
                                            Id = account.Id,
                                            Avatar = account.Avatar,
                                            UserName = account.UserName,
                                            IsOnline = account.IsOnline
                                        }
                                        
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
                throw ex;
            }

        }



        public async Task<int> SendMessage(Chats _get)
        {
            try
            {
                var id = new JWT().GetUserIdFromToken(_get.User.Id);
                var objectId = ObjectId.Parse(_get.Id.ToString());
                var filter = Builders<ChatModelMongoDB>.Filter.Eq(chat => chat.Id, objectId);
                var chatModel = await _customers.Find(filter).FirstOrDefaultAsync();

                
                if (!chatModel.UsersID.Contains(id))
                {
                    return -1;
                }

                

                var lastMessageId = chatModel.Chat?.Count > 0
                    ? chatModel.Chat.OrderByDescending(m => m.CreatedAt).FirstOrDefault()?.Id ?? 0
                    : 0;

                var newMessage = new Chats
                {
                    Id = _get.Id,
                    Message = new Message{
                        Id = lastMessageId + 1,
                        IdUser = id,
                        Text = _get.Message.Text,
                        Img = string.IsNullOrEmpty(_get.Message.Img?.ToString()) ? null : _get.Message.Img.ToString(),
                        IdAnswer = string.IsNullOrEmpty(_get.Message.IdAnswer?.ToString()) ? null : _get.Message.Img.ToString(),
                        CreatedAt = DateTime.UtcNow,
                        View = false,
                        Send = true
                    }
                };

                var update = Builders<ChatModelMongoDB>.Update
                    .Push(chat => chat.Chat, newMessage.Message)
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
                        var ConnectId = await _userConnect.GetUserConnectionId(userId);
                        Console.WriteLine("\n\nUser: " + userId + "Connect id: " + ConnectId);
                        await Clients.Client(ConnectId).SendAsync("ReceiveMessage", newMessage);
                    }
                }

                return lastMessageId;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<List<Message>> GetMessage(Chats _get)
        {
            try
            {
                var id = new JWT().GetUserIdFromToken(_get.User.Id);
                var user = await context.User.FirstOrDefaultAsync(u => u.Id == id);
                if (user != null)
                {
                    var objectId = ObjectId.Parse(_get.Id.ToString());
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
                throw ex;
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
                throw ex;
            }
        }


        public async Task<bool> ViewMessage(Chats _get)
        {
            try
            {
                var objectId = ObjectId.Parse(_get.Id.ToString());
                var filter = Builders<ChatModelMongoDB>.Filter.Eq(chat => chat.Id, objectId);
                var chatModel = await _customers.Find(filter).FirstOrDefaultAsync();
                if (chatModel != null && chatModel.UsersID.Contains(_get.User.Id))
                {
                    foreach (var message in chatModel.Chat)
                    {
                        if (message.IdUser == _get.User.Id && message.View == false)
                        {
                            message.View = true;
                        }
                    }
                    var update = Builders<ChatModelMongoDB>.Update.Set(chat => chat.Chat, chatModel.Chat);
                    await _customers.UpdateOneAsync(filter, update);
                    foreach (var userId in chatModel.UsersID)
                    {
                        if (userId == _get.User.Id)
                        {
                            var ConnectId = await _userConnect.GetUserConnectionId(userId);
                            await Clients.Client(ConnectId).SendAsync("ViewMessage", _get.Id);
                        }
                    }
                    return true;
                }
                Console.WriteLine("Chat ID not found in user IDs.");
                return false;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}