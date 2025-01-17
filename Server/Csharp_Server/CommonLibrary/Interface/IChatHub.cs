using PGAdminDAL;
using Messages.Models.MessageChat;

namespace Messages.Interface
{
    public interface IChatHub
    {
        Task<bool> Connect(string token);
        Task<List<Chats>> disconnection(string token);
        Task Update(string token);
        Task<Chats> CreateChat(Chats _get);
        Task<List<Chats>> GetChats(string token);
        Task<int> SendMessage(Chats _get);
        Task<List<Message>> GetMessage(Chats _get);
        Task<string> GetId(string token);
        Task<bool> ViewMessage(Chats _get);
    }
}
