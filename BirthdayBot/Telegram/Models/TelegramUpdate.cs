using System.Text.Json.Serialization;

namespace BirthdayBot.Telegram.Models
{
    public class TelegramUpdate
    {
        [JsonPropertyName("update_id")]
        public int UpdateId { get; set; }
        [JsonPropertyName("message")]
        public TelegramMessage Message { get; set; }
    }
}