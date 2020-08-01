
using Newtonsoft.Json;
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
    /// 微信域名检测接口--免费的
    /// </summary>
    public class WXUrlCheck : IHttpHandler
    {
        private const int DURATION = 24 * 60;
        private static string userIP = "127.0.0.1";
        protected const string GET = "GET";
        private string wxCheckApiKey = ConfigTool.ReadVerifyConfig("wxCheckApiKey", "CheckKey");
        private TimeSpan _strWorkingDayAM = DateTime.Parse("08:00").TimeOfDay;//工作时间上午08:00
        private TimeSpan _strWorkingDayPM = DateTime.Parse("21:00").TimeOfDay;
        public void ProcessRequest(HttpContext context)
        {
            string result = string.Empty;
            if (context.Request.HttpMethod.ToUpper().Equals(GET))
            {
                userIP = GetWebClientIp(context);
                context.Response.ContentType = "text/plain";
                TimeSpan dspNow = DateTime.Now.TimeOfDay;

                string urlCheck = "{\"State\":false,\"Code\":\"003\",\"Data\":\" \",\"Msg\":\"参数错误,联系管理员!\"}";

                if (IsInTimeInterval(dspNow, _strWorkingDayAM, _strWorkingDayPM))
                {

                    if (!string.IsNullOrEmpty(context.Request["url"]))
                    {
                        //需要检测的网址
                        urlCheck = context.Request["url"]; //检测的值

                        if (!urlCheck.ToLower().Contains(".kuaizhan.com") && !urlCheck.ToLower().Contains(".luckhl8.com") && !urlCheck.ToLower().Contains(".kennethhwong.cn") && !urlCheck.ToLower().Contains(".hatai678.top") && !urlCheck.ToLower().Contains(".jszkgs.top"))
                        {
                            ServiceApiClient SpVoiceObj2 = null;
                            //    ServiceApiClient SpVoiceObj = null;
                            try
                            {

                                bool isTrue = urlCheck.StartsWith("http");
                                if (!isTrue) { urlCheck = "http://" + urlCheck; }
                                if (urlCheck.StartsWith("http://") || urlCheck.StartsWith("https://"))
                                {
                                    urlCheck = System.Web.HttpUtility.UrlEncode(urlCheck);
                                }

                                string json2 = "{\"Mode\":\"AuthKey\",\"Param\":\"{\'CheckUrl\':\'" + urlCheck + "\',\'UserKey\':\'" + wxCheckApiKey + "\',\'UserIP\':\'" + userIP + "\',\'IsFreeKey\':1}\"}";

                                SpVoiceObj2 = new ServiceApiClient("NetTcpBinding_IServiceApi");
                                SpVoiceObj2.Open();
                                result = SpVoiceObj2.Api(json2);
                                SpVoiceObj2.Close();

                                Logger.WriteLogggerTest("#################################################");
                                Logger.WriteLogggerTest(userIP + ":" + result);
                                Logger.WriteLogggerTest(context.Request.ServerVariables["HTTP_X_FORWARDED_FOR"]);



                            }
                            catch (System.ServiceModel.CommunicationException)
                            {
                                //  if (SpVoiceObj != null) SpVoiceObj.Abort();
                                if (SpVoiceObj2 != null) SpVoiceObj2.Abort();
                            }
                            catch (TimeoutException)
                            {
                                //   if (SpVoiceObj != null) SpVoiceObj.Abort();
                                if (SpVoiceObj2 != null) SpVoiceObj2.Abort();
                            }
                            catch (Exception ex)
                            {
                                //   if (SpVoiceObj != null) SpVoiceObj.Abort();
                                if (SpVoiceObj2 != null) SpVoiceObj2.Abort();
                                result = "{\"State\":false,\"Code\":\"003\",\"Data\":\"" + urlCheck + "\",\"Msg\":\"请求操作在配置的超时,请联系管理员!\"}";
                                LogTools.WriteLine(userIP + ":" + ex.Message);
                            }
                        }
                        else
                        {
                            result = "{\"State\":false,\"Code\":\"002\",\"Data\":\"" + urlCheck + " \",\"Msg\":\"歇一歇,访问太快了,联系管理员!\"}";
                        }


                    }
                    else
                    {
                        result = "{\"State\":false,\"Code\":\"003\",\"Data\":\"" + userIP + "\",\"Msg\":\"参数错误,联系管理员!\"}";

                    }
                }
                else
                {
                    result = "{\"State\":false,\"Code\":\"003\",\"Data\":\"" + userIP + "\",\"Msg\":\"测试接口,请在每天(08:00-21:00)时间段进行测试,需要讨论技术,联系管理员.\"}";
                }

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