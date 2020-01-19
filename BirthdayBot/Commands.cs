using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BirthdayBot.Extensions;
using BirthdayBot.Interfaces;

namespace BirthdayBot
{
    public class Commands
    {
        private readonly IAdminManager _adminManager;

        public Commands(IAdminManager adminManager)
        {
            _adminManager = adminManager;
        }
        public bool AdminAdd(ArgumentIterator iter)
        {
            var (hasValue, name) = iter.Advance();
            if (!hasValue)
            {
                Console.WriteLine("Missing username to add as admin.");
                return false;
            }

            if (_adminManager.IsAdmin(name))
            {
                Console.WriteLine($"{name} is already an admin.");
                return false;
            }

            _adminManager.Add(name);
            Console.WriteLine($"{name} is now an admin.");
            Program.SaveConfig();
            return true;
        }
        
        public bool AdminDelete(ArgumentIterator iter)
        {
            var (hasValue, name) = iter.Advance();
            if (!hasValue)
            {
                Console.WriteLine("Missing username to delete from admins.");
                return false;
            }
            
            if (!_adminManager.IsAdmin(name))
            {
                Console.WriteLine($"{name} is not an admin.");
                return false;
            }
            
            _adminManager.Delete(name);
            Console.WriteLine($"{name} is no longer an admin.");
            Program.SaveConfig();
            return true;
        }
        
        public bool AdminList(ArgumentIterator iter)
        {
            Console.WriteLine("Current admins:");
            foreach (var admin in _adminManager.List())
            {
                Console.WriteLine($"* {admin}");
            }

            return true;
        }
        
        public bool TimeZoneIdsWriteToFile(ArgumentIterator iter)
        {
            var timeZoneIds = TimeZoneInfo.GetSystemTimeZones();
            var timezones = new List<string>
            {
                "Time zone ids:"
            };
            timezones.AddRange(timeZoneIds.Select(t => $"* {t.Id}"));

            File.WriteAllLines("timezones.txt", timezones);
            Console.WriteLine("Wrote time zone ids to timezones.txt");
            return true;
        }
        
        
        public bool TimeZoneIdsList(ArgumentIterator iter)
        {
            var timeZoneIds = TimeZoneInfo.GetSystemTimeZones();
            Console.WriteLine("Time zone ids:");
            foreach (var tzId in timeZoneIds)
            {
                Console.WriteLine($"* {tzId.Id}");
            }

            return true;
        }
    }
}