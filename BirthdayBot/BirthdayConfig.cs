using System;
using System.Collections.Generic;

namespace BirthdayBot
{
    public class BirthdayConfig
    {
        public int MessageLoopSleepTimeMs { get; set; }
        public string MessageTemplate { get; set; }
        public string Token { get; set; }
        public string ChatId { get; set; }
        public string TimeZoneId { get; set; }
        public DateTime LastResetDateLocalTime { get; set; }
        public List<string> Admins { get; set; } = new List<string>();
    }
}