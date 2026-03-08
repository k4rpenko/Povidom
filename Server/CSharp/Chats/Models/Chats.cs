namespace Chats.Models
{
    public class ChatsModel
    {
        public string Id { get; set; }
        public List<string> AddUsersIdChat { get; set; }
        public User User { get; set; }
        public Message Message { get; set; }
        public Message[] MessageArray { get; set; }
    }
}
