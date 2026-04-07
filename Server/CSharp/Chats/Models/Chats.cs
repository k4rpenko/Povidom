namespace Chats.Models
{
    public class GeterChatsModel
    {
        public string ChatId { get; set; }
        public string peopleId { get; set; }
        public Message Message { get; set; }
    }
    
    public class ChatsModel
    {
        public string Id { get; set; }
        public User User { get; set; }
        public Message Message { get; set; }
        public Message[] MessageArray { get; set; }
    }
}
