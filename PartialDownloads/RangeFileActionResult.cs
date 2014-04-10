using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management.Instrumentation;
using System.Runtime.Remoting.Messaging;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;
using RangeHelperLib;

namespace RangeResultLib
{
    public class RangeFileActionResult : ActionResult
    {

        private readonly Stream _stream;
        private readonly long _length;
        private readonly string _contentType;
        private readonly string _etag = null;
        public RangeFileActionResult(string filename, string contentType)
        {
            _stream = new FileStream(filename, FileMode.Open, FileAccess.Read);
            _length = _stream.Length;
            _contentType = contentType;

            byte[] hash;
            using (var md5 = MD5.Create())
            {
                hash = md5.ComputeHash(_stream);
            }
            _etag = Convert.ToBase64String(hash);
            _stream.Seek(0, SeekOrigin.Begin);
        }

        public override void ExecuteResult(ControllerContext context)
        {
            using (_stream)
            {
                var ranges = ParseRangeHeader(context.HttpContext.Request);
                if (ranges == null || ranges.Empty)
                {
                    new FileStreamResult(_stream, _contentType).ExecuteResult(context);
                    return;
                }
                if (ranges.Multiple)
                {
                    ProcessMultipleRanges(context, ranges);
                }
                else
                {
                    ProcessSingleRange(context, ranges.Values.Single());
                }
            }
        }


        private void ProcessSingleRange(ControllerContext context, Range range)
        {
            var response = context.HttpContext.Response;
            response.StatusCode = 206;
            response.AddHeader("Content-Range", new ContentRange(range, _length).ToString());
            response.AddHeader("ETag", _etag);
            response.ContentType = _contentType;
            FlushRangeDataInResponse(range, response);
        }

        private void ProcessMultipleRanges(ControllerContext context, Ranges ranges)
        {
            var response = context.HttpContext.Response;
            response.StatusCode = 206;
            response.AddHeader("ETag", _etag);
            response.AddHeader("Content-Type", "multipart/byteranges; boundary=THIS_STRING_SEPARATES");
            foreach (var range in ranges.Values)
            {
                AddRangeInMultipartResponse(context, range);
            }
        }

        private void AddRangeInMultipartResponse(ControllerContext context, Range range)
        {
            var response = context.HttpContext.Response;
            response.Write("-- THIS STRING SEPARATES\x0D\x0A");
            response.Write(string.Format("Content-Type: {0}\x0D\x0A", _contentType));
            var contentRange = new ContentRange(range, _length);
            if (contentRange.IsValid)
            {
                response.Write("Content-Range: " + contentRange + "\x0D\x0A\x0D\x0A");
            }

            FlushRangeDataInResponse(range, response);
            response.Write("\x0D\x0A");
        }

        private void FlushRangeDataInResponse(Range range, HttpResponseBase response)
        {
            var creader = new ChunkReader(_stream);
            ChunkResult result = null;
            var startpos = 0;
            do
            {
                result = creader.GetBytesChunk(range, startpos);
                startpos += result.BytesRead;
                if (result.BytesRead > 0)
                {
                    response.OutputStream.Write(result.Data, 0, result.BytesRead);
                }
                response.Flush();
            } while (result.MoreToRead);
        }


        private Ranges ParseRangeHeader(HttpRequestBase request)
        {
            var rangeHeader = request.Headers["Range"];
            if (string.IsNullOrEmpty(rangeHeader)) return null;
            rangeHeader = rangeHeader.Trim();

            if (!rangeHeader.StartsWith("bytes="))
            {
                return Ranges.InvalidUnit();
            }

            return Ranges.FromBytesUnit(rangeHeader.Substring("bytes=".Length));

        }
    }
}
