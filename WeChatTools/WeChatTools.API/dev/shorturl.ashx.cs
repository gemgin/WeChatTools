using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using WeChatTools.Core;

namespace WeChatTools.API.dev
{
    /// <summary>
    /// 短链接生成接口--正式使用接口
    /// </summary>
    public class shorturl : IHttpHandler
    {
        private const int DURATION = 24 * 60;

        protected const string GET = "GET";
        public void ProcessRequest(HttpContext context)
        {
            string result = string.Empty;
            string url = context.Request["url"];
            if (!string.IsNullOrEmpty(url))
            {

                result = GetICPInfo(url);
                context.Response.ContentType = "text/plain";
 
            }
            else
            {
                result = "未查询到备案信息!！！！！";
            }
            context.Response.Write(result);
            context.Response.End();


        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
        /// <summary>
        /// https://beian.miit.gov.cn 采用官网备案查询
        /// </summary>
        /// <param name="URL"></param>
        /// <returns></returns>
        public static string GetICPInfo(string URL)
        {
            string getString = "";
            if (!IsDomain(URL)) { return getString; }  


            HttpWebRequest httpWebRequest = null;
            HttpWebResponse webResponse = null;
            try
            {
                string domain = "http://127.0.0.1:8039/queryByCondition?domain=" + URL;
                httpWebRequest = (HttpWebRequest)WebRequest.Create(domain);
                httpWebRequest.Host = "127.0.0.1";
                httpWebRequest.Accept = "text/javascript, application/javascript, application/ecmascript, application/x-ecmascript, */*; q=0.01";
                httpWebRequest.KeepAlive = true;

                httpWebRequest.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/77.0.3865.90 Safari/537.36";
                httpWebRequest.Method = "GET";
                httpWebRequest.Timeout = 3000;//建立连接时间
                httpWebRequest.ReadWriteTimeout = 1000;//下载数据时间               

                webResponse = httpWebRequest.GetResponse() as HttpWebResponse;
                getString = Resp2String(webResponse, false);


            }
            catch (Exception ex)
            {
                getString = "";
                //LogTools.WriteLine("执行GetHtml发生错误：" + ex.Message);
                // return getString;
            }
            finally
            {
                if (webResponse != null)
                {
                    webResponse.Close();
                    webResponse = null;
                }
                if (httpWebRequest != null)
                {
                    httpWebRequest.Abort();
                    httpWebRequest = null;
                }
            }


            return getString;
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

        private static string Resp2String(HttpWebResponse HttpWebResponse, bool isZip)
        {
            Stream responseStream = null;
            StreamReader sReader = null;
            String value = null;

            try
            {
                if (!isZip)
                {
                    responseStream = HttpWebResponse.GetResponseStream();
                }
                else
                {
                    //需要Zip解压
                    responseStream = new GZipStream(HttpWebResponse.GetResponseStream(), CompressionMode.Decompress);
                }


                sReader = new StreamReader(responseStream, Encoding.GetEncoding("UTF-8"));
                value = sReader.ReadToEnd();
            }
            catch (Exception ex)
            {

                throw ex;
            }
            finally
            {
                if (sReader != null)
                    sReader.Close();

                if (responseStream != null)
                    responseStream.Close();

                if (HttpWebResponse != null)
                    HttpWebResponse.Close();
            }

            return value;
        }
         
    }
}