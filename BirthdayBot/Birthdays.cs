using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace BirthdayBot
{
    public class Birthdays : IExitHandler, IEnumerable<Birthday>
    {
        private const string BirthdaysFile = "birthdays.json";
        private List<Birthday> _birthdays;
        private readonly IMessagingApi _messagingApi;

        private Birthdays(List<Birthday> birthdays, IMessagingApi messagingApi)
        {
            _birthdays = birthdays;
            _messagingApi = messagingApi;
        }
        
        public void ResetAlerts()
        {
            foreach (var birth in _birthdays)
            {
                birth.ResetAlert();
            }
        }

        public void CongratulateTheUngratulated(DateTime date)
        {
            foreach (var birth in _birthdays.Where(birth => IsBirthday(date, birth)))
            {
                if (_messagingApi.Send($"Happy birthday, {birth.Human}!"))
                {
                    birth.SetAlert();
                    Save();
                }
                else
                {
                    Console.WriteLine($"Failed to happy birthday {birth.Human}");
                }
            }
        }
        
        public static Birthdays Load(IMessagingApi messagingApi)
        {
            var birthdays = JsonHelper.DeserializeFile(BirthdaysFile, new List<Birthday>
            {
                new Birthday("TestHuman", new DateTime(1990,01,01))
            });
            return new Birthdays(birthdays, messagingApi);
        }
        
        public void Save()
        {
            JsonHelper.SerializeToFile(BirthdaysFile, _birthdays);
        }
        
        private static bool IsBirthday(DateTime date, Birthday birthday)
        {
            return birthday.IsHappyBirthday(date) && !birthday.Alerted;
        }

        public void OnExit()
        {
            Save();
        }

        public void Add(Birthday birthday)
        {
            _birthdays.Add(birthday);
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