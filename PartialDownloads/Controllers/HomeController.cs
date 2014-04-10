using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RangeResultLib;

namespace PartialDownloads.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/
        public ActionResult Index()
        {
            return this.RangeFile("~\\Content\\test.png", "image/png");
        }
	}
}