using System;
using System.Collections.Generic;
using System.Net.Http;

namespace BirthdayBot
{
    public class TelegramApi : IMessagingApi
    {
        private readonly string _token;
        private readonly string _chatId;
        private static readonly HttpClient _client = new HttpClient();
        private readonly List<IMessageHandler> _handlers = new List<IMessageHandler>();

        public TelegramApi(string token, string chatId)
        {
            _token = token;
            _chatId = chatId;
        }

        public bool Send(string s)
        {
            try
            {
                var result = _client
                    .GetAsync($"https://api.telegram.org/bot{_token}/sendMessage?chat_id={_chatId}&text={s}").Result;
                if (result.IsSuccessStatusCode)
                    return true;
                return false;
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"[TelegramApi]: Failed to send request: {e}");
                return false;
            }
        }

        public void AddHandler(IMessageHandler handler)
        {
            _handlers.Add(handler);
        }

        public void Fetch()
        {
            var url = $"https://api.telegram.org/bot{_token}/getUpdates?allowed_updates=[\"message\", \"channel_post\"]&offset=-25";
            try
            {
                var result = _client
                    .GetAsync(url).Result;
                if (result.IsSuccessStatusCode)
                {
                    var content = result.Content.ReadAsStringAsync().Result;
                    foreach (var handler in _handlers)
                    {
                        handler.HandleMessage(content);
                    }
                }
                else
                {
                    Console.WriteLine($"[TelegramApi]: Failed to fetch messages: {result.StatusCode}");
                }
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"[TelegramApi]: Failed to fetch messages: {e}");
            }
        }
    }
}