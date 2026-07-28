using Chats.Models;

namespace Chats.Interface
{
    public interface IChatHub
    {
        Task<bool> Connect();
        Task<List<ChatsModel>> disconnection();
        Task<ChatsModel> CreateChat(string AddUsersId);
        Task<List<ChatsModel>> GetChats();
        Task<int> SendMessage(GeterChatsModel _get);
        Task<List<Models.Message>> GetMessage(string chatId);
        Task<bool> ViewMessage(GeterChatsModel _get);
    }
}
