using System.Text.Json.Serialization;

namespace BirthdayBot.Telegram.Models
{
    public class TelegramUser
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("is_bot")]
        public bool IsBot { get; set; }
        [JsonPropertyName("username")]
        public string Username { get; set; }
    }
}