using System;
using System.Collections.Generic;
using System.Text.Json;
using BirthdayBot.IO;
using BirthdayBot.Telegram;
using BirthdayBot.Telegram.Models;
using Moq;
using Xunit;

namespace BirthdayBot.Tests
{
    public class TelegramMessageHandlerTests
    {
        [Fact]
        public void AddingYourOwnBirthdayDoesNotRequireBeingAnAdmin()
        {
            var api = new TelegramApi("", "");
            var birthdays = new Birthdays(new List<Birthday>(), api, Mock.Of<IFileWriter>());
            var sut = new TelegramMessageHandler(api, birthdays, new AdminManager());
            
            var json = CreateTelegramUpdateDto("/birthadd erik 01-01", "EriK", "EriK", 139);
            sut.HandleMessage(json);
            Assert.Contains(birthdays, b =>
                b.Human.Equals("erik", StringComparison.InvariantCultureIgnoreCase) &&
                b.Date.Day == 1 &&
                b.Date.Month == 1);
        }
        
        [Fact]
        public void AddingSomeoneElsesBirthdayWhileNotAnAdminDoesNotAddBirthday()
        {
            var api = new TelegramApi("", "");
            var birthdays = new Birthdays(new List<Birthday>(), api, Mock.Of<IFileWriter>());
            var sut = new TelegramMessageHandler(api, birthdays, new AdminManager());
            
            var json = CreateTelegramUpdateDto("/birthadd tempo 01-01", "EriK", "AnyChat", 139);
            sut.HandleMessage(json);
            Assert.Empty(birthdays);
        }
        
        [Fact]
        public void AddingSomeoneElsesBirthdayWhileAnAdminDoesAddBirthday()
        {
            var api = new TelegramApi("", "");
            var birthdays = new Birthdays(new List<Birthday>(), api, Mock.Of<IFileWriter>());
            var adminManager = new AdminManager(new List<string> {"erik"});
            var sut = new TelegramMessageHandler(api, birthdays, adminManager);
            
            var json = CreateTelegramUpdateDto("/birthadd tempo 12-15", "EriK", "AnyChat", 139);
            sut.HandleMessage(json);
            Assert.Contains(birthdays, b =>
                b.Human.Equals("tempo", StringComparison.InvariantCultureIgnoreCase) &&
                b.Date.Day == 15 &&
                b.Date.Month == 12);
        }

        private static string CreateTelegramUpdateDto(string msg, string sender, string chatUser, int messageId)
        {
            return JsonSerializer.Serialize(new TelegramUpdateDto
            {
                Ok = true,
                Updates = new List<TelegramUpdate>
                {
                    new TelegramUpdate
                    {
                        Message = new TelegramMessage
                        {
                            User = new TelegramUser
                            {
                                Username = sender
                            },
                            MessageId = messageId,
                            Text = msg,
                            Chat = new TelegramChat
                            {
                                Id = 5151,
                                Username = chatUser
                            }
                        },
                        UpdateId = 500
                    }
                }
            });
        }
    }
}