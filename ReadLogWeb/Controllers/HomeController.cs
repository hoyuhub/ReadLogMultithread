using log4net;
using LogRead.Plan_C.Arithmetics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Web;
using System.IO;
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

        FileStream fs = new FileStream("e:\\logfile.log", FileMode.Open, FileAccess.Read);
        public void test()
        {
            ILog log = LogManager.GetLogger("LogError");

            log.Error("文件流长度：" + fs.Length);

        }



    }
}