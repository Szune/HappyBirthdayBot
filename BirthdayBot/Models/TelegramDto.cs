using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace BirthdayBot.Models
{
    public class TelegramUpdateDto
    {
        [JsonPropertyName("ok")]
        public bool Ok { get; set; }
        [JsonPropertyName("result")]
        public List<TelegramUpdate> Updates { get; set; }
        
    }
}