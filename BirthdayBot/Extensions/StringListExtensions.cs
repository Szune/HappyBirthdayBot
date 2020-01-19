using System;
using System.Collections.Generic;
using System.Linq;

namespace BirthdayBot.Extensions
{
    public static class StringListExtensions
    {
        public static bool ContainsIgnoreCase(this List<string> list, string s)
        {
            return list.Any(it => string.Equals(it, s, StringComparison.InvariantCultureIgnoreCase));
        }
        
        public static void RemoveIgnoreCase(this List<string> list, string s)
        {
            var index = list.FindIndex(it => string.Equals(it, s, StringComparison.InvariantCultureIgnoreCase));
            list.RemoveAt(index);
        }
    }
}