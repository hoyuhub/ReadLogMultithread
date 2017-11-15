using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LogRead.Plan_C.Entitys
{
    //提炼日志之后得到的实体
    public class LogEntity
    {
        public LogEntity() { }
        public LogEntity(DateTime time, string json, string url, string hospid, string phone)
        {
            this.time = time;
            this.json = json;
            this.url = url;
            this.hospid = hospid;
            this.phone = phone;
        }
        public string phone { get; set; }
        public DateTime time { get; set; }
        public string hospid { get; set; }
        public string json { get; set; }
        public string url { get; set; }
    }
}
