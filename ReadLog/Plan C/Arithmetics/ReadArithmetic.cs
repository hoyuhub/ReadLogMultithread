using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using Newtonsoft.Json;
using System.IO.MemoryMappedFiles;
using LogRead.Plan_C.Entitys;
using System.Threading;
using ReadLog;
using ReadLog.Plan_C.Arithmetics;

namespace LogRead.Plan_C.Arithmetics
{
    public static class ReadArithmetic
    {

        //日志文件路路径,传入或者以配置文件的形式读取
        private static string path = System.Web.Configuration.WebConfigurationManager.AppSettings["LogFilePath"];

        /// <summary>
        /// 从指定位置
        /// 获取一百行数据
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <returns></returns>
        public static void ListLine()
        {
            if (File.Exists(path))
            {
                using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
                {
                    //设置线程池中最多有20个线程
                    ThreadPool.SetMaxThreads(20, 20);
                    //获取上次文件流结束位置（若旧流位置大于新流长度则视为新文件，流位置初始化）
                    RedisDal dal = new RedisDal();
                    long oldPosition = dal.GetFilePosition();
                    if (oldPosition > fs.Length)
                    {
                        fs.Position = 0L;
                    }
                    else
                    {
                        fs.Position = oldPosition;
                    }
                    //每100行文本创建一个线程，最后余量创建一个线程
                    using (StreamReader sr = new StreamReader(fs, Encoding.UTF8))
                    {
                        string line = string.Empty;
                        List<string> listStr = new List<string>();
                        while (true)
                        {

                            line = sr.ReadLine();
                            if (line != null)
                            {
                                if (listStr.Count == 100)
                                {
                                    AnalyticalArithmetic anal = new AnalyticalArithmetic();
                                    anal.listStr = listStr.ToList();
                                    ThreadPool.QueueUserWorkItem(new WaitCallback(anal.AllFun));
                                    listStr.Clear();
                                }
                                listStr.Add(line);
                            }
                            else
                            {
                                break;
                            }

                        }
                        AnalyticalArithmetic analytickal = new AnalyticalArithmetic();
                        //保存旧流位置
                        dal.SetFilePosition(fs.Length.ToString());
                        dal.Dispose();
                        analytickal = new AnalyticalArithmetic();
                        analytickal.listStr = listStr.ToList();
                        ThreadPool.QueueUserWorkItem(new WaitCallback(analytickal.AllFun));
                    }
                }
            }

        }
    




    }
}
