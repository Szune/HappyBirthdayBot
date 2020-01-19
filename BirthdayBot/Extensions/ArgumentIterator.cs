using System.Collections.Generic;
using System.Linq;

namespace BirthdayBot.Extensions
{
    public class ArgumentIterator
    {
        private readonly List<string> _array;
        private int _index = 0;

        public ArgumentIterator(IEnumerable<string> array)
        {
            _array = array.ToList();
        }

        public (bool HasValue, string Value) Advance()
        {
            if (_index > _array.Count - 1)
            {
                return (false, "");
            }

            return (true, _array[_index]);
        }
    }
}