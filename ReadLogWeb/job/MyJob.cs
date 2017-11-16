using log4net;
using LogRead.Plan_C.Arithmetics;
using Quartz;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace ReadLogWeb.job
{
    public class MyJob : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            Stopwatch st = new Stopwatch();
            st.Start();
            ReadArithmetic.ListLine();
            st.Stop();
            ILog log = LogManager.GetLogger("LogError");
            TimeSpan ts = st.Elapsed;
            log.Error("这一次用时：" + ts);
        }
    }
}