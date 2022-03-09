using System;
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
                    if (LogTools.IsInTimeInterval(dspNow, _strWorkingDayAM, _strWorkingDayPM) && !string.IsNullOrEmpty(url))
                    {
                        userIP = LogTools.GetWebClientIp(context);

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


         
    }
}