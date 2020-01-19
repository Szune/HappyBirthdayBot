using System;
using System.Linq;

namespace BirthdayBot.Extensions
{
    public static class StringArrayExtensions
    {
        public static bool DispatchCommand(this string[] arr, params (string Path, Predicate<ArgumentIterator> Action)[] commands)
        {
            foreach (var command in commands)
            {
                var parts = GetParts(command.Path);
                if (arr.Take(parts.Length).Select(p => p.ToUpperInvariant()).SequenceEqual(parts.Select(p => p.ToUpperInvariant())))
                {
                    return command.Action(new ArgumentIterator(arr.Skip(parts.Length)));
                }
            }

            return default;
        }
        
        public static bool DispatchCommand(this string[] arr, params (string Path, Action<ArgumentIterator> Action)[] commands)
        {
            foreach (var command in commands)
            {
                var parts = GetParts(command.Path);
                if (arr.Take(parts.Length).Select(p => p.ToUpperInvariant()).SequenceEqual(parts.Select(p => p.ToUpperInvariant())))
                {
                    command.Action(new ArgumentIterator(arr.Skip(parts.Length)));
                    return true;
                }
            }

            return false;
        }

        private static string[] GetParts(string path)
        {
            return path.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        }
    }
}