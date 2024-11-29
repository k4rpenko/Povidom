namespace Server.Models.MessageChat
{
    public class SendChats
    {
        public string? Avatar {  get; set; }
        public string? NickName { get; set; }
        public string? CreatorId { get; set; }
        public string? ChatId { get; set; }
        public LastMessage? LastMessage { get; set; }
        public DateTime? CreatedAt { get; set; }

        public bool? Send { get; set; }
        public bool? View { get; set; }
    }

    public class LastMessage
    {
        public string? UserId { get; set; }
        public string? Message { get; set; }
        public bool? View { get; set; } = false;
    }
}
