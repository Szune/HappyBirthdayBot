using System.Collections.Generic;
using BirthdayBot.Extensions;
using BirthdayBot.Interfaces;

namespace BirthdayBot
{
    public class AdminManager : IAdminManager
    {
        private readonly List<string> _admins;
        
        public AdminManager()
        {
            _admins = new List<string>();
        }

        public AdminManager(List<string> admins)
        {
            _admins = admins ?? new List<string>();
        }

        public bool IsAdmin(string username)
        {
            return _admins.ContainsIgnoreCase(username);
        }

        public void Add(string username)
        {
            _admins.Add(username);
        }

        public void Delete(string username)
        {
            _admins.RemoveIgnoreCase(username);
        }

        public IEnumerable<string> List()
        {
            return new List<string>(_admins);
        }
    }
}