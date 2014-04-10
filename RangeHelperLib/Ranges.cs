using System.Collections.Generic;

namespace RangeHelperLib
{
    public class Ranges
    {

        public IEnumerable<Range> Values { get { return _values; } }

        public bool Empty { get { return _values.Count == 0; } }

        public bool Multiple { get { return _values.Count > 1; } }

        private readonly List<Range> _values;
        public Ranges()
        {
            _values = new List<Range>();
        }


        private void Add(Range item)
        {
            _values.Add(item);
        }

        public static Ranges InvalidUnit()
        {
            return new Ranges();
        }

        public static Ranges FromBytesUnit(string value)
        {
            var tokens = value.Split(',');

            var ranges = new Ranges();
            foreach (var token in tokens)
            {
                ranges.Add(new Range(token));
            }

            return ranges;
        }
    }
}