using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RangeHelperLib
{
    public class ChunkReader
    {
        private readonly Stream _stream;
        public ChunkReader(Stream stream)
        {
            _stream = stream;
        }


        public ChunkResult GetBytesChunk(Range range, int startpos)
        {
            var chunk = new ChunkResult();
            var reader = new BinaryReader(_stream);
            var remainingLen = range.Length != -1 ? range.Length - startpos : -1;
            if (remainingLen == 0)
            {
                return new ChunkResult();
            }
                
            var bytesWanted = remainingLen != -1 ? Math.Min(1024, remainingLen) : 1024;
            reader.BaseStream.Seek(range.FromBegin ? startpos : range.From + startpos, SeekOrigin.Begin);
            var buffer = new byte[bytesWanted];
            chunk.BytesRead = reader.Read(buffer, 0, (int)bytesWanted);
            chunk.Data = buffer;
            chunk.MoreToRead = remainingLen != -1
                ? chunk.BytesRead != remainingLen
                : chunk.BytesRead != 0;

            return chunk;
        }
    }
}
