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
        private static string path = "e:\\logfile.log";

        //文件流读取起始位置
        private static long position = 0L;


        #region 线程一

        /// <summary>
        /// 从指定位置
        /// 获取一百行数据
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <returns></returns>
        public static void ListLine()
        {
            RedisDal dal = new RedisDal();
            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
                fs.Position = position;
                using (StreamReader sr = new StreamReader(fs, Encoding.UTF8))
                {
                    string line = string.Empty;
                    List<string> listStr = new List<string>();
                    Dictionary<string, List<string>> dic = new Dictionary<string, List<string>>();

                    while (true)
                    {
                        try
                        {
                            line = sr.ReadLine();
                            if (line != null)
                            {
                                if (listStr.Count == 100)
                                {
                                    AnalyticalArithmetic anal = new AnalyticalArithmetic();
                                    anal.dal = dal;
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
                        catch (Exception e)
                        {
                            throw e;
                        }
                    }
                    AnalyticalArithmetic analytickal = new AnalyticalArithmetic();

                    position = fs.Length;
                    analytickal = new AnalyticalArithmetic();
                    analytickal.listStr = listStr.ToList();
                    analytickal.dal = dal;
                    ThreadPool.QueueUserWorkItem(analytickal.AllFun);
                }
            }

        }
        #endregion




    }
}
