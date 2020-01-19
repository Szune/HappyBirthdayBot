using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BirthdayBot.Interfaces;
using BirthdayBot.IO;

namespace BirthdayBot
{
    public class Birthdays : IExitHandler, IEnumerable<Birthday>
    {
        private const string BirthdaysFile = "birthdays.json";
        private List<Birthday> _birthdays;
        private readonly IMessagingApi _messagingApi;
        private readonly IFileWriter _fileWriter;

        public Birthdays(List<Birthday> birthdays, IMessagingApi messagingApi, IFileWriter fileWriter)
        {
            _birthdays = birthdays;
            _messagingApi = messagingApi;
            _fileWriter = fileWriter;
        }
        
        public void ResetAlerts()
        {
            foreach (var birth in _birthdays)
            {
                birth.ResetAlert();
            }
        }

        public void CongratulateTheUngratulated(DateTime date, string messageTemplate)
        {
            foreach (var birth in _birthdays.Where(birth => IsBirthday(date, birth)))
            {
                if (_messagingApi.Send(messageTemplate.Replace("{user}", $"@{birth.Human}")))
                {
                    birth.SetAlert();
                    Save();
                }
                else
                {
                    Console.WriteLine($"Failed to happy birthday @{birth.Human}");
                }
            }
        }
        
        public static Birthdays Load(IMessagingApi messagingApi)
        {
            var birthdays = JsonHelper.DeserializeFile(BirthdaysFile, new List<Birthday>
            {
                new Birthday("TestHuman", new DateTime(1990,01,01))
            });
            return new Birthdays(birthdays, messagingApi, new FileWriter());
        }
        
        public void Save()
        {
            _fileWriter.SerializeToFile(BirthdaysFile, _birthdays);
        }
        
        private static bool IsBirthday(DateTime date, Birthday birthday)
        {
            return birthday.IsHappyBirthday(date) && !birthday.Alerted;
        }

        public void OnExit()
        {
            Save();
        }

        public void AddOrUpdate(Birthday birthday)
        {
            var index = _birthdays.FindIndex(b =>
                string.Equals(b.Human, birthday.Human, StringComparison.InvariantCultureIgnoreCase));
            if (index > -1)
            {
                _birthdays[index] = birthday;
            }
            else
            {
                _birthdays.Add(birthday);
            }
            Save();
        }

        public bool Remove(string name)
        {
            var index = _birthdays.FindIndex(b => b.Human.ToUpperInvariant() == name.ToUpperInvariant());
            if (index > -1)
            {
                _birthdays.RemoveAt(index);
                Save();
                return true;
            }

            return false;
        }

        public IEnumerator<Birthday> GetEnumerator()
        {
            return _birthdays.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}