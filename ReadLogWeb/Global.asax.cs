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
using Quartz;
using Quartz.Impl;
using ReadLogWeb.job;
using Quartz.Impl.Triggers;

namespace ReadLogWeb
{
    public class MvcApplication : System.Web.HttpApplication
    {
        IScheduler sched;
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            log4net.Config.XmlConfigurator.Configure();
            //MyJobs();  
        }

        public void MyJobs()
        {

            //工厂
            ISchedulerFactory factory = new StdSchedulerFactory();
            //启动
            IScheduler scheduler = factory.GetScheduler();
            scheduler.Start();
            //描述工作
            IJobDetail jobDetail = new JobDetailImpl("mylittlejob", null, typeof(MyJob));
            //触发器
            ISimpleTrigger trigger = new SimpleTriggerImpl("mytrigger",
                null,
                DateTime.Now,
                null,
                SimpleTriggerImpl.RepeatIndefinitely,
                TimeSpan.FromSeconds(10));
            //执行
            scheduler.ScheduleJob(jobDetail, trigger);

        }

        private void OnTimedEvent(object sender, ElapsedEventArgs e)
        {
            lock (this)
            {
                Stopwatch st = new Stopwatch();
                st.Start();
                ReadArithmetic.ListLine();
                ILog log = LogManager.GetLogger("LogError");
                TimeSpan ts = st.Elapsed;
                log.Error("这一次用时：" + ts);
            }
        }
    }
}
