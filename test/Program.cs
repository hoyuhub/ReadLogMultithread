using System;
using System.IO;
using System.Collections.Generic;

namespace test
{
    class Program
    {
        public string path = "e:\\logfile.log";
        public static void Main(string[] args)
        {
            string path = "e:\\logfile.log";
            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                Console.WriteLine("length：" + fs.Length);
                Console.WriteLine("position:" + fs.Position);
                using (StreamReader sr = new StreamReader(fs))
                {
                    string line = sr.ReadLine();
                    Console.WriteLine("length：" + fs.Length);
                    Console.WriteLine("position:" + fs.Position);
                }
            }
        }


        public void CheckLine(long oldLength)
        {
            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                using (StreamReader sr = new StreamReader(fs))
                {
                    fs.Position = oldLength;
                    string line = string.Empty;
                    while ((line = sr.ReadLine()) != null)
                    {


                    }
                }

            }

        }


        public void CheckLength()
        {
            long length = 0L;
            List<long> list = new List<long>();
            if (File.Exists(path))
            {
                using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    int i = 0;
                    while (true)
                    {
                        if (length != fs.Length)
                        {
                            length = fs.Length;
                            if (length % 101 != 0)
                            {
                                Console.WriteLine("已经查询了：" + i);
                                Console.WriteLine(length);
                            }
                            i = 0;
                        }
                        i++;
                    }

                }
            }


        }
    }
}
