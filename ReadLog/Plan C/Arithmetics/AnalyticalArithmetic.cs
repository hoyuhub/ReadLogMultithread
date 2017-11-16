using LogRead.Plan_C.Entitys;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ReadLog.Plan_C.Arithmetics
{
    //本类用于多线程使用，解析接收到的文本数据
    public class AnalyticalArithmetic
    {

        //在这里执行数据库插入操作
        private RedisDal dal = new RedisDal();
        public List<string> listStr { get; set; }

        /// <summary>
        /// 根据得到的文本返回符合要求的数据集合
        /// </summary>
        /// <param name="list">获取到的文本</param>
        /// <returns></returns>
        public List<LogEntity> GetLogEntitys()
        {
            lock (this)
            {
                List<LogEntity> logList = new List<LogEntity>();
                //定义正则表达式
                string pattern = @"(\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2}),\d{3} \[\d+] [A-Z]+ .* - Request .* (\{.*}) ([a-z]+|[a-z]+/[a-z]+)";

                foreach (string str in listStr)
                {
                    Match match = Regex.Match(str, pattern);
                    //解析符合表达式的内容添加到集合里
                    if (match.Groups.Count > 1)
                    {
                        Dictionary<string, object> dic = new Dictionary<string, object>();
                        string hospid = string.Empty;
                        string phone = string.Empty;
                        dic = JsonConvert.DeserializeObject<Dictionary<string, object>>(match.Groups[2].ToString());
                        if (dic.Keys.Contains("request"))
                        {
                            Dictionary<string, string> dicRequest = JsonConvert.DeserializeObject<Dictionary<string, string>>(dic["request"].ToString());
                            if (dicRequest.Keys.Contains("hospid"))
                            {
                                hospid = dicRequest["hospid"];
                            }
                            if (dicRequest.Keys.Contains("phone"))
                            {
                                phone = dicRequest["phone"];

                            }
                        }
                        logList.Add(new LogEntity(Convert.ToDateTime(match.Groups[1].Value), match.Groups[2].Value, match.Groups[3].Value, hospid, phone));
                    }
                }
                return logList;
            }
        }

        public void AllFun(object s)
        {
            lock (this)
            {
                //根据问本行公共区的到需要处理的数据，并清空公共区（消费）
                List<LogEntity> list = GetLogEntitys();

                HospPhoneStatistics(list);
                //执行获取手机短讯接口调用查询方法
                PhoneSendCounts(list);
                dal.Dispose();
            }
        }

        /// <summary>
        /// 统计每家医院每个接口每秒的调用次数
        /// 统计每家医院每个接口每天的调用次数
        /// </summary>
        public void HospPhoneStatistics(List<LogEntity> list)
        {
            lock (this)
            {
                #region 用linq将数据分组处理
                var result =
                   list.GroupBy(
                        e => e.url,
                         (url, urlGroup) => new
                         {
                             url,
                             hospGroups = urlGroup
                             .GroupBy(
                                e2 => e2.hospid,
                                 (hospid, hospGroup) => new
                                 {
                                     hospid,
                                     timeGroups = hospGroup
                                     .OrderBy(e3 => e3.time)
                                     .GroupBy(e3 => e3.time)
                                     .Select(g => new { time = g.Key, count = g.Count() })
                                 }
                                 ).Select(e4 => new
                                 {
                                     hospid = e4.hospid,
                                     timeGroups = e4.timeGroups,
                                     timeGroupsCount = e4.timeGroups.Count()
                                 }
                                        )
                         }
                         ).Select(s => new
                         {
                             url = s.url,
                             hospGroups = s.hospGroups
                         }
                               );
                #endregion

                //每家医院每个接口没秒的调用次数
                List<Count> listSecondCount = new List<Count>();

                //每家医院每个接口每天的调用次数
                List<Count> listDayCount = new List<Count>();

                //将linq结果添加到对应的集合中
                foreach (var e in result)
                {
                    foreach (var e2 in e.hospGroups)
                    {
                        foreach (var e3 in e2.timeGroups)
                        {
                            //将按秒分组的数据取出 判断集合中是否已经有当前类型数据，有则追加，无则添加
                            listSecondCount.Add(new Count(e.url, e2.hospid, e3.count, e3.time));
                            bool thisSecond = false;
                            foreach (var c in listSecondCount)
                            {
                                if (c.time == e3.time && c.hospid == e2.hospid && c.url == e.url)
                                {
                                    thisSecond = true;
                                    c.count += e3.count;
                                    break;
                                }
                            }
                            if (!thisSecond)
                            {
                                listSecondCount.Add(new Count(e.url, e2.hospid, e3.count, e3.time));
                            }


                            string daytime = e3.time.Year + "-" + e3.time.Month + "-" + e3.time.Day;
                            //获取每个医院每个接口当天的调用次数
                            bool thisday = false;
                            foreach (Count c in listDayCount)
                            {
                                if (c.daytime == daytime && c.hospid == e2.hospid && c.url == e.url)
                                {
                                    thisday = true;
                                    c.count += e3.count;
                                    break;
                                }
                            }
                            if (!thisday)
                            {
                                listDayCount.Add(new Count(e.url, e2.hospid, e3.count, daytime));
                            }
                        }

                    }
                }

                dal.DayCount(listDayCount);
                dal.SecondCount(listSecondCount);
                dal.SecondTopCount(listSecondCount);
            }
        }

        //按手机号统计一段时间内短讯发送接口的调用此时
        //时间单位：分钟
        public void PhoneSendCounts(List<LogEntity> list)
        {
            lock (this)
            {

                //每个手机号每分钟调用指定接口的次数
                List<Count> listPhoneCount = new List<Count>();
                foreach (LogEntity l in list)
                {
                    if (string.Compare(l.url, "send") == 0)
                    {

                        string minuteTime = l.time.ToString("yyyy/MM/dd HH:mm:00");
                        bool thisMinute = false;
                        foreach (Count c in listPhoneCount)
                        {
                            string time = c.time.ToString("yyyy/MM/dd HH:mm:00");
                            if (string.Compare(time, minuteTime) == 0 && string.Compare(c.phone, l.phone) == 0)
                            {
                                c.count += 1;
                                thisMinute = true;
                                break;
                            }
                        }
                        if (!thisMinute)
                        {
                            if (!string.IsNullOrEmpty(l.phone))
                                listPhoneCount.Add(new Count(l.phone, Convert.ToDateTime(minuteTime), 1));
                        }
                    }

                }
                dal.PhoneMinuteCount(listPhoneCount);
                dal.PhoneMinuteTopCount(listPhoneCount);
            }
        }

    }
}
