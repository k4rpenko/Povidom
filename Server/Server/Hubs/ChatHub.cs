using Amazon.Runtime.Internal;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using NoSQL;
using PGAdminDAL;
using Server.Controllers;
using Server.Models.MessageChat;
using Server.Protection;
using System;
using System.Diagnostics.Metrics;
using System.Linq;

namespace Server.Hubs
{
    public class ChatHub : Hub
    {
        private readonly IMongoCollection<ChatModelMongoDB> _customers;
        private readonly AppDbContext context;

        public ChatHub(AppMongoContext _Mongo, IConfiguration _configuration, AppDbContext _context)
        {
            _customers = _Mongo.Database?.GetCollection<ChatModelMongoDB>(_configuration.GetSection("MongoDB:MongoDbDatabaseChat").Value);
            context = _context;
        }


        public async Task<GetChats> CreateChat(ChatModel _chat)
        {
            if (_chat == null || _chat.AddUsersIdChat == null || _chat.AddUsersIdChat.Count < 2)
            {
                throw new ArgumentException("Invalid chat model or user IDs.");
            }

            var id = new JWT().GetUserIdFromToken(_chat.AddUsersIdChat[0]);

            var filter = Builders<ChatModelMongoDB>.Filter.And(
                Builders<ChatModelMongoDB>.Filter.Or(
                    Builders<ChatModelMongoDB>.Filter.Eq(chat => chat.UsersID[0], id),
                    Builders<ChatModelMongoDB>.Filter.Eq(chat => chat.UsersID[0], _chat.AddUsersIdChat[1])
                ),
                Builders<ChatModelMongoDB>.Filter.Or(
                    Builders<ChatModelMongoDB>.Filter.Eq(chat => chat.UsersID[1], id),
                    Builders<ChatModelMongoDB>.Filter.Eq(chat => chat.UsersID[1], _chat.AddUsersIdChat[1])
                )
            );

            var You = await context.User.FirstOrDefaultAsync(u => u.Id == id);
            var People = await context.User.FirstOrDefaultAsync(u => u.Id == _chat.AddUsersIdChat[1]);

            if (You == null || People == null)
            {
                throw new Exception("One or both users not found.");
            }

            var PeopleInfo = new GetChats
            {
                LastMessage = "",
                Avatar = People.Avatar,
                NickName = People.FirstName
            };

            var existingChat = await _customers.Find(filter).FirstOrDefaultAsync();

            if (existingChat != null)
            {
                PeopleInfo.ChatId = existingChat.Id.ToString();
                return PeopleInfo;
            }

            _chat.AddUsersIdChat[0] = id;
            var newChat = new ChatModelMongoDB
            {
                UsersID = _chat.AddUsersIdChat,
                Timestamp = DateTime.UtcNow
            };

            var YouInfo = new GetChats
            {
                ChatId = newChat.UsersID.ToString(),
                LastMessage = "",
                Avatar = You.Avatar,
                NickName = You.FirstName
            };

            foreach (var userId in newChat.UsersID)
            {
                await Clients.User(userId).SendAsync("CreatChat", YouInfo);
            }

            await _customers.InsertOneAsync(newChat);
            PeopleInfo.ChatId = newChat.Id.ToString();

            You.ChatsID.Add(newChat.Id.ToString());
            People.ChatsID.Add(newChat.Id.ToString());

            await context.SaveChangesAsync();
            return PeopleInfo;
        }

        public async Task<bool> SendMessage(ChatModel _chat)
        {
            try
            {

                var objectId = ObjectId.Parse(_chat.IdChat.ToString());
                var filter = Builders<ChatModelMongoDB>.Filter.Eq(chat => chat.Id, objectId);
                var chatModel = await _customers.Find(filter).FirstOrDefaultAsync();

                if (!chatModel.UsersID.Contains(_chat.CreatorId))
                {
                    Console.WriteLine("Chat ID not found in user IDs.");
                    return false;
                }

                var id = new JWT().GetUserIdFromToken(_chat.CreatorId);

                var lastMessageId = chatModel.Chat?.Count > 0
                    ? chatModel.Chat.OrderByDescending(m => m.CreatedAt).FirstOrDefault()?.Id ?? 0
                    : 0;

                var newMessage = new Message
                {
                    Id = lastMessageId + 1,
                    IdUser = id,
                    Text = _chat.Text,
                    Img = string.IsNullOrEmpty(_chat.Img?.ToString()) ? null : _chat.Img.ToString(),
                    IdAnswer = string.IsNullOrEmpty(_chat.Answer?.ToString()) ? null : _chat.Img.ToString(),
                    CreatedAt = DateTime.UtcNow,
                    View = false,
                    Send = true
                };

                var update = Builders<ChatModelMongoDB>.Update
                    .Push(chat => chat.Chat, newMessage)
                    .Set(chat => chat.Timestamp, DateTime.UtcNow);

                var updateResult = await _customers.UpdateOneAsync(filter, update);

                if (updateResult.MatchedCount == 0)
                {
                    return false;
                }

                var Return = new GetChats
                {
                    Send = true
                };

                foreach (var userId in chatModel.UsersID)
                {
                    if (userId != id)
                    {
                        await Clients.User(userId).SendAsync("ReceiveMessage", newMessage);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending message: {ex.Message}");
                return false;
            }
        }

        public async Task<List<GetChats>> Connect(string token)
        {
            try
            {
                var id = new JWT().GetUserIdFromToken(token);
                var user = await context.User.FirstOrDefaultAsync(u => u.Id == id);
                if (user != null)
                {
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

        public async Task<List<GetChats>> StopConnect(string token)
        {
            try
            {
                var id = new JWT().GetUserIdFromToken(token);
                var user = await context.User.FirstOrDefaultAsync(u => u.Id == id);
                if (user != null)
                {
                    user.IsOnline = false;
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

        public async Task<List<GetChats>> GetChats(TokenModel token)
        {
            try
            {
                var id = new JWT().GetUserIdFromToken(token.token);
                var user = await context.User.FirstOrDefaultAsync(u => u.Id == id);
                if (user != null)
                {
                    if (user.ChatsID != null && user.ChatsID.Count > 0)
                    {
                        Console.WriteLine("user.ChatsID.Count: " + user.ChatsID.Count);
                        var ChatsData = new List<GetChats>();
                        foreach (var item in user.ChatsID)
                        {
                            Console.WriteLine(item);
                        }
                        foreach (var GetChatID in user.ChatsID)
                        {
                            var objectIds = user.ChatsID.Select(chatId => ObjectId.Parse(GetChatID.ToString())).ToList();
                            var filter = Builders<ChatModelMongoDB>.Filter.In(chat => chat.Id, objectIds);

                            var chatModels = await _customers.Find(filter).ToListAsync();


                            foreach (var chatModel in chatModels)
                            {
                                foreach (var userId in chatModel.UsersID)
                                {
                                    if (userId != id)
                                    {
                                        var Ac = await context.User.FirstOrDefaultAsync(u => u.Id == userId);
                                        var chatInfo = new GetChats
                                        {
                                            ChatId = chatModel.Id.ToString(),
                                            LastMessage = chatModel.Chat.Count > 0 ? chatModel.Chat[chatModel.Chat.Count - 1].Text : "",
                                            Avatar = Ac?.Avatar,
                                            NickName = Ac?.UserName
                                        };

                                        ChatsData.Add(chatInfo);
                                    }
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

    }
}