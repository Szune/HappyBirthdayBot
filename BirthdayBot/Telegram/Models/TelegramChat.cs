using System.Text.Json.Serialization;

namespace BirthdayBot.Telegram.Models
{
    public class TelegramChat
    {
        [JsonPropertyName("id")]
        public long Id { get; set; }
        [JsonPropertyName("username")]
        public string Username { get; set; }
    }
}