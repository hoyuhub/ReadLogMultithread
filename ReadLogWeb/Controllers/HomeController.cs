using log4net;
using LogRead.Plan_C.Arithmetics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace ReadLogWeb.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public string test()
        {
            string length = string.Empty;
            string path = "e:\\logfile.log";
            if (System.IO.File.Exists(path))
            {
                using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read,FileShare.ReadWrite))
                {
                    ILog log = LogManager.GetLogger("LogError");
                    log.Error("文件流长度：" + fs.Length);
                    length = fs.Length.ToString();
                }
            }
            return length;

        }



    }
}