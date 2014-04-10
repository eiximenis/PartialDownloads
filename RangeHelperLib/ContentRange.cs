using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RangeHelperLib
{
    public class ContentRange
    {
        private readonly long _start;
        private readonly long _len;
        private readonly long _total;

        public ContentRange(Range range, long total)
        {
            _start = range.FromBegin ? 0 : range.From;
            _len = range.Length != -1 ? range.Length : total;
            _total = total;
        }

        public bool IsValid
        {
            get { return _len != -1; }
        }

        public override string ToString()
        {
            return string.Format("bytes {0}-{1}/{2}"
                , _start, _len, _total != -1 ? _total.ToString(CultureInfo.InvariantCulture) : "*");
        }
    }
}
