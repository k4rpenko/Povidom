using Chats.Models;

namespace Chats.Interface
{
    public interface IChatHub
    {
        Task<bool> Connect();
        Task<List<ChatsModel>> disconnection();
        Task Update();
        Task<ChatsModel> CreateChat(ChatsModel _get);
        Task<List<ChatsModel>> GetChats();
        Task<int> SendMessage(ChatsModel _get);
        Task<List<Models.Message>> GetMessage(ChatsModel _get);
        Task<bool> ViewMessage(ChatsModel _get);
    }
}
