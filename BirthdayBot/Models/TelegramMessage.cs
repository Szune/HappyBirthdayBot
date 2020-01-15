using System.Text.Json.Serialization;

namespace BirthdayBot.Models
{
    public class TelegramMessage
    {
        [JsonPropertyName("message_id")]
        public int MessageId { get; set; }
        [JsonPropertyName("text")]
        public string Text { get; set; }
        [JsonPropertyName("from")]
        public TelegramUser User { get; set; }
        [JsonPropertyName("chat")]
        public TelegramChat Chat { get; set; }
    }
}