namespace Server.Models.MessageChat
{
    public class GetChats
    {
        public string? Avatar {  get; set; }
        public string? NickName { get; set; }

        public string? ChatId { get; set; }
        public string? LastMessage { get; set; }
        public DateTime? CreatedAt { get; set; }

        public bool? Send { get; set; }
        public bool? View { get; set; }
    }
}
