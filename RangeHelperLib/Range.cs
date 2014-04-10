namespace RangeHelperLib
{
    public class Range
    {
        private readonly long _from;
        private readonly long _to;

        public long Length
        {
            get
            {
                if (_from != -1 && _to != -1)
                {
                    return (_to - _from) + 1;
                }
                if (_from == -1 && _to != -1)
                {
                    return _to;
                }

                return -1;
            }
        }

        public long From
        {
            get { return _from; }
        }

        public long To
        {
            get { return _to; }
        }

        public bool FromBegin
        {
            get { return _from == -1 || _from == 0; }
        }

        public bool ToEnd
        {
            get { return _to == -1; }
        }

        public Range(string value)
        {
            if (!value.Contains("-"))
            {
                _from = long.Parse(value);
                _to = -1;
            }
            else if (value.StartsWith("-"))
            {
                _from = -1;
                _to = long.Parse(value.Substring(1));
            }
            else
            {
                var idx = value.IndexOf('-');
                _from = long.Parse(value.Substring(0, idx));
                _to = idx == value.Length - 1 ? -1 :
                    long.Parse(value.Substring(idx + 1));
            }
        }
    }
}