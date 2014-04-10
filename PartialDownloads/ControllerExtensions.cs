using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RangeResultLib;

namespace PartialDownloads
{
    internal static class ControllerExtensions
    {
        public static RangeFileActionResult RangeFile(this ControllerBase controller, string filename,
            string contenttype)
        {
            var fullname = filename.StartsWith("~") ? 
                controller.ControllerContext.HttpContext.Server.MapPath(filename) : 
                filename;

            return new RangeFileActionResult(fullname, contenttype);
        }
    }
}