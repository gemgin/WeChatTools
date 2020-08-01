using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using WeChatTools.Core;

namespace WeChatTools.API.pro
{
    /// <summary>
    /// 短链接生成接口--测试接口
    /// </summary>
    public class shorturl3 : IHttpHandler
    {

        private static string userIP = "127.0.0.1";
        protected const string POST = "POST";
        private string shorturlkey = ConfigTool.ReadVerifyConfig("shorturlKey", "CheckKey");
        private TimeSpan _strWorkingDayAM = DateTime.Parse("08:00").TimeOfDay;//工作时间上午08:00
        private TimeSpan _strWorkingDayPM = DateTime.Parse("21:00").TimeOfDay;

        public void ProcessRequest(HttpContext context)
        {
            string result = string.Empty;
            if (context.Request.HttpMethod.ToUpper().Equals(POST))
            {
                string url = context.Request["url"];
                string type = context.Request["type"]; //key ,md5值
                string model = context.Request["model"]; //a,还原;b.生成
                context.Response.ContentType = "text/plain";
                TimeSpan dspNow = DateTime.Now.TimeOfDay;

                if (!string.IsNullOrEmpty(model) && model.Equals("b"))
                {  //生成短链接
                    if (IsInTimeInterval(dspNow, _strWorkingDayAM, _strWorkingDayPM) && !string.IsNullOrEmpty(url))
                    {
                        userIP = GetWebClientIp(context);

                        ServiceApiClient SpVoiceObj = null;
                        try
                        {

                            if (type.ToUpper() != "URLCN" && type.ToUpper() != "WURLCN" && type.ToUpper() != "TCN")
                            {
                                type = "URLCN";
                            }
                            if (url.StartsWith("http://") || url.StartsWith("https://"))
                            {
                                url = System.Web.HttpUtility.UrlEncode(url);
                            }
                            
                            string json2 = "{\"Mode\":\"ShortUrl\",\"Param\":\"{\'CheckUrl\':\'" + url + "\',\'type\':\'" + type + "\',\'UserKey\':\'" + shorturlkey + "\',\'UserIP\':\'" + userIP + "\',\'IsFreeKey\':1}\"}";

                            SpVoiceObj = new ServiceApiClient("NetTcpBinding_IServiceApi");
                            SpVoiceObj.Open();
                            result = SpVoiceObj.Api(json2);
                            SpVoiceObj.Close();


                            if (!string.IsNullOrEmpty(context.Request.QueryString["callback"]))
                            {
                                string callBack = context.Request.QueryString["callback"].ToString(); //回调
                                result = callBack + "(" + result + ")";
                            }
                        }
                        catch (System.ServiceModel.CommunicationException)
                        {
                            if (SpVoiceObj != null) SpVoiceObj.Abort();
                        }
                        catch (TimeoutException)
                        {
                            if (SpVoiceObj != null) SpVoiceObj.Abort();
                        }
                        catch (Exception ex)
                        {
                            if (SpVoiceObj != null) SpVoiceObj.Abort();
                            result = "{\"State\":false,\"Code\":\"003\",\"Data\":\"https://url.cn/5mfnDv7\",\"Msg\":\"请求操作在配置的超时,请联系管理员!\"}";
                            LogTools.WriteLine(shorturlkey + ":" + ex.Message);
                        }



                    }
                    else
                    {
                        result = "{\"State\":false,\"Code\":\"003\",\"Data\":\"https://url.cn/5mfnDv7\",\"Msg\":\"测试接口,请在每天(08:00-21:00)时间段进行测试,需要讨论技术,联系管理员.\"}";

                    }
                }
                else
                { //短链接还原
                    result = HttpHelper.GetLocation(url);
                }


            }
            else
            {
                result = "{\"State\":false,\"Code\":\"003\",\"Data\":\"https://url.cn/5mfnDv7\",\"Msg\":\"参数错误,联系管理员!\"}";
            }
            context.Response.Headers.Add("Access-Control-Allow-Origin", "https://www.rrbay.com");
            context.Response.Headers.Add("Access-Control-Allow-Methods", "POST");
            context.Response.Headers.Add("Access-Control-Allow-Credentials", "true");

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



        private bool IsInTimeInterval(TimeSpan time, TimeSpan startTime, TimeSpan endTime)
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
    }
}