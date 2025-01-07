using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace WeChatTools.Core
{
    public class LogTools
    {
        private static string logFileName ;
        private static string formate = "["+FormatNowDateTimeToString()+"]  " ;

        public static void Init()
        {
            string newFileName = FormatNowDateTimeToString("yyy-MM-dd") + ".txt";
          //  string absolutePath = System.Web.HttpContext.Current.Server.MapPath("~/Logs/" + newFileName);
            string absolutePath = System.AppDomain.CurrentDomain.BaseDirectory + "/Logs/" + newFileName;
            if (!File.Exists(absolutePath))
            {
                CreateFile(absolutePath);
            }
            else
            {
                EditorFile(absolutePath);
            }
            logFileName = absolutePath;
        }

        private static string getFormate() 
        {
            return "[" + FormatNowDateTimeToString() + "]  ";
        }
        #region 格式化时间
        /// <summary>
        /// 格式化当前时间为字符串  默认格式为：yyyy-MM-dd HH:mm:ss  24小时制
        /// </summary>
        /// <param name="formate">时间格式</param>
        /// <returns></returns>
        public static string FormatNowDateTimeToString(string formate = "yyyy-MM-dd HH:mm:ss")
        {
            return DateTime.Now.ToString(formate);
        }
        #endregion 
        /// <summary>
        /// 创建文件
        /// </summary>
        /// <param name="fileName"></param>
        public static void CreateFile(string fileName)
        {
            if (!File.Exists(fileName))
            {
                FileStream myFs = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite);
                myFs.Close();
            }
        }
        /// <summary>
        /// 编辑文件
        /// </summary>
        /// <param name="fileName"></param>
        public static void EditorFile(string fileName)
        {
            if (File.Exists(fileName))
            {
                FileStream myFs = new FileStream(fileName, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
                myFs.Close();
            }
        }
        /// <summary>
        /// 追加一条信息
        /// </summary>
        /// <param name="text"></param>
        public static void Write(string text)
        {
            Init();
            using (StreamWriter sw = new StreamWriter(logFileName, true, Encoding.UTF8))
            {
                sw.Write(getFormate()+ text);
                sw.Flush();
                sw.Dispose();
            }
        }
        /// <summary>
        /// 追加一条信息
        /// </summary>
        /// <param name="logFile"></param>
        /// <param name="text"></param>
        public static void Write(string logFile, string text)
        {
            Init();
            using (StreamWriter sw = new StreamWriter(logFileName, true, Encoding.UTF8))
            {
                sw.Write(getFormate()+ text);
                sw.Flush();
                sw.Dispose();
            }
        }
        /// <summary>
        /// 追加一行信息
        /// </summary>
        /// <param name="text"></param>
        public static void WriteLine(string text)
        {
            Init();
            text += "\r\n";
            using (StreamWriter sw = new StreamWriter(logFileName, true, Encoding.UTF8))
            {
                sw.Write(getFormate()+ text);
                sw.Flush();
                sw.Dispose();
            }
        }
        /// <summary>
        /// 追加一行信息
        /// </summary>
        /// <param name="logFile"></param>
        /// <param name="text"></param>
        public static void WriteLine(string logFile, string text)
        {
            Init();
            text += "\r\n";
            using (StreamWriter sw = new StreamWriter(logFileName, true, Encoding.UTF8))
            {
                sw.Write(getFormate()+ text);
                sw.Flush();
                sw.Dispose();
            }
        }


        /// <summary>
        /// 判断一个字符串，是否匹配指定的表达式(区分大小写的情况下)
        /// </summary>
        /// <param name="expression">正则表达式</param>
        /// <param name="str">要匹配的字符串</param>
        /// <returns></returns>
        public static bool IsMatch(string expression, string str)
        {
            Regex reg = new Regex(expression);
            if (string.IsNullOrEmpty(str))
                return false;
            return reg.IsMatch(str);

        }

        /// <summary>
        /// 验证字符串是否是域名
        /// </summary>
        /// <param name="str">指定字符串</param>
        /// <returns></returns>
        public static bool IsDomain(string str)
        {
            if (str.ToLower().Equals("gov.cn") || str.ToLower().Equals("org.cn"))
            {
                return false;
            }
            else
            {
                string pattern = @"^[a-zA-Z0-9][-a-zA-Z0-9]{0,62}(\.[a-zA-Z0-9][-a-zA-Z0-9]{0,62})+$";
                return IsMatch(pattern, str);
            }
        }


        public static string GetWebClientIp(HttpContext httpContext)
        {
            string customerIP = "127.0.0.1";

            if (httpContext == null || httpContext.Request == null || httpContext.Request.ServerVariables == null) return customerIP;

            customerIP = httpContext.Request.ServerVariables["HTTP_CDN_SRC_IP"];

            if (String.IsNullOrWhiteSpace(customerIP) || "unknown".Equals(customerIP.ToLower()))
            {

                customerIP = httpContext.Request.ServerVariables["Proxy-Client-IP"];
            }
            if (String.IsNullOrWhiteSpace(customerIP) || "unknown".Equals(customerIP.ToLower()))
            {

                customerIP = httpContext.Request.ServerVariables["WL-Proxy-Client-IP"];
            }
            /*
            if (String.IsNullOrWhiteSpace(customerIP) || "unknown".Equals(customerIP.ToLower()))
            {

                customerIP = httpContext.Request.ServerVariables["HTTP_VIA"];
            }
            */
            if (String.IsNullOrWhiteSpace(customerIP) || "unknown".Equals(customerIP.ToLower()))
            {

                customerIP = httpContext.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                if (!String.IsNullOrWhiteSpace(customerIP) && customerIP.Contains(","))
                {
                    string[] xx = customerIP.Split(new char[] { ',' });
                    if (xx.Length > 2)
                    {
                        customerIP = xx[xx.Length - 1].Trim();
                    }
                    else
                    {
                        customerIP = xx[0];

                    }
                }
            }
            if (String.IsNullOrWhiteSpace(customerIP))
            {

                customerIP = httpContext.Request.ServerVariables["HTTP_CLIENT_IP"];
                if (!String.IsNullOrWhiteSpace(customerIP) && customerIP.Contains(","))
                {
                    customerIP = customerIP.Split(new char[] { ',' })[0];
                }
            }
            if (String.IsNullOrWhiteSpace(customerIP))
            {

                customerIP = httpContext.Request.ServerVariables["REMOTE_ADDR"];
                if (!String.IsNullOrWhiteSpace(customerIP) && customerIP.Contains(","))
                {
                    customerIP = customerIP.Split(new char[] { ',' })[0];
                }

            }

            if (!IsIP(customerIP))
            {
                customerIP = "127.0.0.1";
            }
            return customerIP;
        }

        /// <summary>
        /// 检查IP地址格式
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public static bool IsIP(string ip)
        {
            if (!String.IsNullOrWhiteSpace(ip))
            {
                return System.Text.RegularExpressions.Regex.IsMatch(ip, @"^((2[0-4]\d|25[0-5]|[01]?\d\d?)\.){3}(2[0-4]\d|25[0-5]|[01]?\d\d?)$");
            }
            else
            {
                return false;
            }

        }

        /// <summary>
        /// URL过滤特殊字符
        /// </summary>
        /// <param name="url">URL</param>
        /// <returns></returns>
        public static string FilterUrl(string url)
        {
            string replaceStr = url;
            if (!string.IsNullOrEmpty(url))
            {
                replaceStr = replaceStr.ToLower();
                replaceStr = replaceStr.Replace("script", "");
                replaceStr = replaceStr.Replace("delete", "");
                replaceStr = replaceStr.Replace("alert", "");
                replaceStr = replaceStr.Replace("select", "");
                replaceStr = replaceStr.Replace("update", "");
                replaceStr = replaceStr.Replace("insert", "");
              //  replaceStr = replaceStr.Replace("like", "");
            }

            return replaceStr;

        }

        public static bool IsInTimeInterval(TimeSpan time, TimeSpan startTime, TimeSpan endTime)
        {
            //判断时间段开始时间是否小于时间段结束时间,如果不是就交换
            if (startTime > endTime)
            {
                TimeSpan tempTime = startTime;
                startTime = endTime;
                endTime = tempTime;
            }

            if (time > startTime && time < endTime)
            {
                return true;
            }
            return false;
        }

    }
    /// <summary>
    /// 验证项目Token
    /// </summary>
    public class Logger
    {
        #region=============日志处理==============
        //在网站根目录下创建日志目录
        public static string path = System.Web.HttpRuntime.AppDomainAppPath.ToString() + "/Logs";

        /**
      * 实际的写日志操作
      * @param type 日志记录类型
      * @param className 类名
      * @param content 写入内容
      */
        public static void WriteLoggger(string content)
        {
            if (!Directory.Exists(path))//如果日志目录不存在就创建
            {
                Directory.CreateDirectory(path);
            }

            string time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");//获取当前系统时间
            string filename = path + "/Logtxt_" + DateTime.Now.ToString("yyyy-MM-dd") + ".log";//用日期对日志文件命名

            //创建或打开日志文件,向日志文件末尾追加记录
            StreamWriter mySw = File.AppendText(filename);

            //向日志文件写入内容
            string write_content = time + ": " + content;
            mySw.WriteLine(write_content);

            //关闭日志文件
            mySw.Close();
        }

        /**
        * 实际的写日志操作
        * @param type 日志记录类型
        * @param className 类名
        * @param content 写入内容
        */
        public static void WriteLogggerTest(string content)
        {
            if (!Directory.Exists(path))//如果日志目录不存在就创建
            {
                Directory.CreateDirectory(path);
            }

            string time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");//获取当前系统时间
            string filename = path + "/LogtxtTest_" + DateTime.Now.ToString("yyyy-MM-dd") + ".log";//用日期对日志文件命名

            //创建或打开日志文件,向日志文件末尾追加记录
            StreamWriter mySw = File.AppendText(filename);

            //向日志文件写入内容
            string write_content = time + ": " + content;
            mySw.WriteLine(write_content);

            //关闭日志文件
            mySw.Close();
        }
        #endregion

    }
}
