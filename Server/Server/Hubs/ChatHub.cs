using Amazon.Runtime.Internal;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
using MongoDB.Driver;
using NoSQL;
using PGAdminDAL;
using Server.Controllers;
using Server.Models.MessageChat;
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


        public async Task<string> CreateChat(ChatModel _chat)
        {
            var filter = Builders<ChatModelMongoDB>.Filter.And(
                Builders<ChatModelMongoDB>.Filter.Eq(chat => chat.UsersID[0], _chat.AddUsersIdChat[0]),
                Builders<ChatModelMongoDB>.Filter.Eq(chat => chat.UsersID[1], _chat.AddUsersIdChat[1])
            );

            var existingChat = await _customers.Find(filter).FirstOrDefaultAsync();

            if (existingChat != null)
            {
                return existingChat.Id.ToString();
            }

            var newChat = new ChatModelMongoDB
            {
                UsersID = _chat.AddUsersIdChat,
                Timestamp = DateTime.UtcNow
            };

            await _customers.InsertOneAsync(newChat);
            return newChat.Id.ToString();
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


                var lastMessageId = chatModel.Chat?.Count > 0
                    ? chatModel.Chat.OrderByDescending(m => m.CreatedAt).FirstOrDefault()?.Id ?? 0
                    : 0;

                var newMessage = new Message
                {
                    Id = lastMessageId + 1,
                    IdUser = _chat.CreatorId,
                    Text = _chat.Text,
                    Img = string.IsNullOrEmpty(_chat.Img?.ToString()) ? null : _chat.Img.ToString(),
                    IdAnswer = string.IsNullOrEmpty(_chat.Answer?.ToString()) ? null : _chat.Img.ToString(),
                    CreatedAt = DateTime.UtcNow
                };

                var update = Builders<ChatModelMongoDB>.Update
                    .Push(chat => chat.Chat, newMessage)
                    .Set(chat => chat.Timestamp, DateTime.UtcNow);

                var updateResult = await _customers.UpdateOneAsync(filter, update);

                if (updateResult.MatchedCount == 0)
                {
                    return false;
                }

                foreach (var userId in chatModel.UsersID)
                {
                    if (userId != _chat.CreatorId)
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
                if (user != null) {
                    if (user.Status)
                    {
                        List<GetChats> ChatsData = new List<GetChats>();
                        foreach (var chatId in user.ChatId)
                        {
                            var objectId = ObjectId.Parse(chatId.ToString());
                            var filter = Builders<ChatModelMongoDB>.Filter.Eq(chat => chat.Id, objectId);
                            var chatModel = await _customers.Find(filter).FirstOrDefaultAsync();

                            foreach (var userId in chatModel.UsersID)
                            {
                                if (userId != id)
                                {
                                    await Clients.User(userId).SendAsync("Status", true);

                                    var Ac = await context.User.FirstOrDefaultAsync(u => u.Id == userId);

                                    var chatInfo = new GetChats
                                    {
                                        ChatId = chatModel.Id.ToString(),
                                        LastMessage = chatModel.Chat[chatModel.Chat.Count - 1].Text,
                                        Avatar = Ac.Avatar,
                                        NickName = Ac.UserName
                                    };
                                }
                            }
                        }
                        return ChatsData;
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
    }
}
