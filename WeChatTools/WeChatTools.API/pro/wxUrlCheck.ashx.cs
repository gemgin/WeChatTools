using System;
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
        private TimeSpan _strWorkingDayAM = DateTime.Parse("09:00").TimeOfDay;//工作时间上午08:00
        private TimeSpan _strWorkingDayPM = DateTime.Parse("20:00").TimeOfDay;
        public void ProcessRequest(HttpContext context)
        {
            string result = string.Empty;
            if (context.Request.HttpMethod.ToUpper().Equals(GET))
            {
                userIP = LogTools.GetWebClientIp(context);
                context.Response.ContentType = "text/plain";
                TimeSpan dspNow = DateTime.Now.TimeOfDay;

                string urlCheck = "{\"State\":false,\"Code\":\"003\",\"Data\":\" \",\"Msg\":\"参数错误,联系管理员!\"}";

                if (LogTools.IsInTimeInterval(dspNow, _strWorkingDayAM, _strWorkingDayPM))
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
                                string apiMode = context.Request["mode"]; //检测的值
                                if (string.IsNullOrEmpty(apiMode))
                                {
                                    apiMode = "AuthKey";
                                }
                                string json2 = "{\"Mode\":\'" + apiMode + "\',\"Param\":\"{\'CheckUrl\':\'" + urlCheck + "\',\'UserKey\':\'" + wxCheckApiKey + "\',\'UserIP\':\'" + userIP + "\',\'IsFreeKey\':1}\"}";

                                SpVoiceObj2 = new ServiceApiClient("NetTcpBinding_IServiceApi");
                                SpVoiceObj2.Open();
                                result = SpVoiceObj2.Api(json2);
                                SpVoiceObj2.Close();


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
                    result = "{\"State\":false,\"Code\":\"003\",\"Data\":\"" + userIP + "\",\"Msg\":\"测试接口,请在每天(09:00-20:00)时间段进行测试,需要讨论技术,联系管理员.\"}";
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

 

    }
}