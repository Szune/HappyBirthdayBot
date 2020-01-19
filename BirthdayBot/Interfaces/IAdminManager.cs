using System.Collections.Generic;

namespace BirthdayBot.Interfaces
{
    public interface IAdminManager
    {
        bool IsAdmin(string username);
        void Add(string username);
        void Delete(string username);
        IEnumerable<string> List();
    }
}