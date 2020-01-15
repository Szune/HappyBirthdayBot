using System;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using BirthdayBot.Models;

namespace BirthdayBot
{
    public class TelegramMessageHandler : IMessageHandler, IExitHandler
    {
        private readonly TelegramApi _telegramApi;
        private readonly Birthdays _birthdays;
        private readonly LimitedStore<int> _storedIds;

        public TelegramMessageHandler(TelegramApi telegramApi, Birthdays birthdays)
        {
            _storedIds = JsonHelper.DeserializeFile("messages.json", new LimitedStore<int>());
            _telegramApi = telegramApi;
            _birthdays = birthdays;
        }

        public void HandleMessage(string message)
        {
            var msg = JsonSerializer.Deserialize<TelegramUpdateDto>(message,
                new JsonSerializerOptions {IgnoreNullValues = true});
            if (!msg.Ok || msg.Updates.Count < 1)
                return;

            foreach (var m in msg.Updates)
            {
                if (_storedIds.Contains(m.Message.MessageId))
                    continue;

                _storedIds.Push(m.Message.MessageId);
                Console.WriteLine(
                    $"Received message '{m.Message.Text}' from {m.Message.User.Username} in {m.Message.Chat.Id} ({m.Message.Chat.Username ?? "No name"})");
                HandleNewMessage(m);
            }
        }

        private void HandleNewMessage(TelegramUpdate m)
        {
            var parts = m.Message.Text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            switch (parts[0].Trim().ToUpperInvariant())
            {
                case "/BIRTHCOMMANDS":
                    ListCommands();
                    break;
                case "/BIRTHQUIT":
                    if (!IsAdmin(m))
                        break;
                    _telegramApi.Send("Exiting BirthdayBot");
                    Program.OnExit();
                    break;
                case "/BIRTHADD":
                    if (!IsAdmin(m))
                        break;
                    AddBirthday(parts);
                    break;
                case "/BIRTHDELETE":
                    if (!IsAdmin(m))
                        break;
                    DeleteBirthday(parts);
                    break;
                case "/BIRTHLIST":
                    ListBirthdays();
                    break;
            }
        }

        private static bool IsAdmin(TelegramUpdate m)
        {
            return Program.Config.Admins.Contains(m.Message.User.Username?.ToUpperInvariant().Trim() ?? "");
        }

        /// <summary>
        /// Handles messages like /birthlist
        /// </summary>
        private void ListBirthdays()
        {
            var birthdays = string.Join("\n", 
                _birthdays.Select(b => $"{b.Human} is happy birthday on {b.Date:MM-dd}"));
            _telegramApi.Send("-Birthday list-\n" + birthdays);
        }

        /// <summary>
        /// Handles messages like /birthcommands
        /// </summary>
        private void ListCommands()
        {
            string[] commands =
            {
                "-Command list-",
                "List commands: /birthcommands",
                "List birthdays: /birthlist",
                "Add birthday [admin]: /birthadd name MM-dd",
                "Delete birthday [admin]: /birthdelete name",
                "Quit BirthdayBot [admin]: /birthquit"
            };
            _telegramApi.Send(string.Join("\n", commands));
        }

        /// <summary>
        /// Handles messages like /birthdelete name
        /// </summary>
        private void DeleteBirthday(string[] parts)
        {
            try
            {
                var name = parts[1].Trim();
                if (_birthdays.Remove(name))
                {
                    _telegramApi.Send($"Birthday deleted: {name}");
                }
                else
                {
                    _telegramApi.Send($"Birthday does not exist for: {name}");
                }
            }
            catch
            {
                _telegramApi.Send("Failed to delete birthday, expected command to be in format: /birthdelete name");
            }
        }

        /// <summary>
        /// Handles messages like /birthadd name 01-05
        /// </summary>
        private void AddBirthday(string[] parts)
        {
            try
            {
                var name = parts[1].Trim();
                var birthdate = parts[2].Trim();

                var birthday = new Birthday(name,
                    DateTime.ParseExact(birthdate, "MM-dd", DateTimeFormatInfo.InvariantInfo));
                _birthdays.Add(birthday);
                _telegramApi.Send($"Birthday added: {name} on {birthday.Date:MM-dd}");
            }
            catch
            {
                _telegramApi.Send("Failed to add birthday, expected command to be in format: /birthadd name MM-dd");
            }
        }

        public void OnExit()
        {
            JsonHelper.SerializeToFile("messages.json", _storedIds);
        }
    }
}