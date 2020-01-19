using System;
using System.Globalization;
using System.Linq;
using System.Text.Json;
using BirthdayBot.Extensions;
using BirthdayBot.Interfaces;
using BirthdayBot.IO;
using BirthdayBot.Telegram.Models;

namespace BirthdayBot.Telegram
{
    public class TelegramMessageHandler : IMessageHandler, IExitHandler
    {
        private readonly TelegramApi _telegramApi;
        private readonly Birthdays _birthdays;
        private readonly IAdminManager _adminManager;
        private readonly LimitedStore<int> _storedIds;

        public TelegramMessageHandler(TelegramApi telegramApi, Birthdays birthdays, IAdminManager adminManager)
        {
            _storedIds = JsonHelper.DeserializeFile("messages.json", new LimitedStore<int>(25));
            _telegramApi = telegramApi;
            _birthdays = birthdays;
            _adminManager = adminManager;
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
            var user = m?.Message?.User?.Username ?? "";
            var msg = m?.Message?.Text ?? "";
            var parts = msg.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length < 1)
            {
                return;
            }
            switch (parts[0].Trim().ToUpperInvariant())
            {
                case "/BIRTHCOMMANDS":
                    ListCommands();
                    break;
                case "/BIRTHQUIT":
                    if (!IsAdmin(user))
                        break;
                    _telegramApi.Send("Exiting BirthdayBot");
                    Program.OnExit();
                    break;
                case "/BIRTHADD":
                    AddBirthday(parts, user);
                    break;
                case "/BIRTHDELETE":
                    if (!IsAdmin(user))
                        break;
                    DeleteBirthday(parts);
                    break;
                case "/BIRTHLIST":
                    ListBirthdays();
                    break;
            }
        }

        private bool IsAdmin(string username)
        {
            return _adminManager.IsAdmin(username);
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
                "Add birthday [admin when adding others]: /birthadd name MM-dd",
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
        private void AddBirthday(string[] parts, string senderUsername)
        {
            try
            {
                var nameToAdd = parts[1].Trim();
                if (IsAdmin(senderUsername) || 
                    string.Equals(nameToAdd, senderUsername, StringComparison.InvariantCultureIgnoreCase))
                {
                    var birthdate = parts[2].Trim();

                    var birthday = new Birthday(nameToAdd,
                        DateTime.ParseExact(birthdate, "MM-dd", DateTimeFormatInfo.InvariantInfo));
                    _birthdays.AddOrUpdate(birthday);
                    _telegramApi.Send($"Birthday added: {nameToAdd} on {birthday.Date:MM-dd}");
                }
                else if (!IsAdmin(senderUsername))
                {
                    _telegramApi.Send("You need to be an admin to add someone else");
                }
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