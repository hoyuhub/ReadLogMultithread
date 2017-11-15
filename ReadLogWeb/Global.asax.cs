using LogRead.Plan_C.Arithmetics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Timers;
using System.Diagnostics;
using ReadLog;
using log4net;

namespace ReadLogWeb
{
    public class MvcApplication : System.Web.HttpApplication
    {

        private long position = 0L;

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            log4net.Config.XmlConfigurator.Configure();

            //OnTimedEvent();
            //System.Timers.Timer myTimer = new System.Timers.Timer(30000);
            //myTimer.Elapsed += new System.Timers.ElapsedEventHandler(OnTimedEvent);
            //myTimer.Interval = 60000;
            //myTimer.Enabled = true;

        }


        //private void OnTimedEvent(object source,ElapsedEventArgs e)
        //private void OnTimedEvent()
        //{
        //    Stopwatch st = new Stopwatch();
        //    st.Start();
        //    Arithmetic ar = new Arithmetic(position);
        //    Thread t1 = new Thread(new ThreadStart(ar.ListLine));
        //    Thread t2 = new Thread(new ThreadStart(ar.HospStatistics));
        //    t1.Start();
        //    t2.Start();
        //    t2.Join();
        //    position = ar.Position;
        //    st.Stop();
        //    ILog log = LogManager.GetLogger("LogError");
        //    TimeSpan ts = st.Elapsed;
        //    log.Error("这一次用时：" + ts);
        //}
    }
}
