using System;
using System.IO;
using System.Text;

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
