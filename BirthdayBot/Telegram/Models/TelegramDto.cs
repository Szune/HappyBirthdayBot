using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace BirthdayBot.Telegram.Models
{
    public class TelegramUpdateDto
    {
        [JsonPropertyName("ok")]
        public bool Ok { get; set; }
        [JsonPropertyName("result")]
        public List<TelegramUpdate> Updates { get; set; }
        
    }
}