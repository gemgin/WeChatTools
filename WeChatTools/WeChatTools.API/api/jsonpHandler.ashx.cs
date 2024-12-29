using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;
using WeChatTools.Core;


namespace WeChatTools.API
{
    /// <summary>
    /// 微信域名检测授权api
    /// </summary>
    public class jsonpHandler : IHttpHandler
    {
        protected const string S_PROC_NAME = "sProcName";              //调用方法名
 
        protected const string DOMAIN_KEY = "domainKey";               //微信域名检测key
        protected const string OPEN_ID = "openId";                     //用户openid
        protected const string PIN_LV = "pinLv";                       //检测频率
        protected const string COUNT_KEY = "countKey";                 //不限频率，流量计费模式，请求次数
        protected const string AUTH_TIME = "authTime";                 //检测创建时间
        protected const string AUTH_END_TIME = "authEndTime";          //授权截至时间
        protected const string AUTH_REMARK = "authRemark";             //授权备注一般用户qq

        protected const string POST = "POST";
        public void ProcessRequest(HttpContext context)
        {
            ///获取调用方法名
            string sProName = context.Request[S_PROC_NAME];
            string result = string.Empty;
            Dictionary<string, string> dic = new Dictionary<string, string>();
            if (context.Request.HttpMethod.ToUpper().Equals(POST))
            {
                #region 获取post中的参数
                ///获取POST参数集合
                NameValueCollection nvcForm = context.Request.Form;
                ///遍历POST参数集合
                foreach (string key in nvcForm.Keys)
                {
                    //将key value 参数加入到Dictionary对象中
                    dic.Add(key, nvcForm[key]);
                }
                #endregion

                try
                {
       

                    #region 获取授权key信息

                    string wxcheckKey = string.Empty;
                    switch (sProName)
                    {
                        case "Insert":

                            wxcheckKey = "wxcheck:" + dic[DOMAIN_KEY];
                            if (RedisCacheToolsYUN0.Exists(wxcheckKey))
                            {
                                RedisCacheToolsYUN0.Remove(wxcheckKey);
                            }
                            if (Convert.ToInt32(dic[PIN_LV]) == 0)
                            {
                                if (!RedisCacheToolsYUN0.Exists("keyCount:" + wxcheckKey))
                                {
                                    //初始化，初始化30个检测次数
                                    RedisCacheToolsYUN0.Add("keyCount:" + wxcheckKey, 30, DateTime.Parse(dic[AUTH_END_TIME]));
                                }
                            }
                            else
                            {
                                if (RedisCacheToolsYUN0.Exists("keyCount:" + wxcheckKey))
                                {
                                    RedisCacheToolsYUN0.Remove("keyCount:" + wxcheckKey);
                                }
                            }
                            string keyValue = dic[OPEN_ID] + ";" + Convert.ToInt32(dic[PIN_LV]);
                            RedisCacheToolsYUN0.Add(wxcheckKey, keyValue, DateTime.Parse(dic[AUTH_END_TIME]));
                            result = "{\"Status\":\"100\",\"Msg\":\"授权key成功!\"}";

                            break;
                        case "UpdateCount":
                            string wxKey = dic[DOMAIN_KEY];
                            int wxKeyCount = Convert.ToInt32(dic[COUNT_KEY]);
                            string endDateTime = dic[AUTH_END_TIME];
                            wxcheckKey = "wxcheck:" + wxKey;
                            if (RedisCacheToolsYUN0.Exists(wxcheckKey))
                            {
                                int keyCountValue = 0;
                                if (RedisCacheToolsYUN0.Exists("keyCount:" + wxcheckKey))
                                {
                                    keyCountValue = RedisCacheToolsYUN0.Get<int>("keyCount:" + wxcheckKey);

                                }
                                keyCountValue = keyCountValue + wxKeyCount;
                                DateTime dt = DateTime.Parse(endDateTime);
                                // DateTime dt = DateTime.Now.AddMonths(12);//更新一次 延长12个月
                                RedisCacheToolsYUN0.Add("keyCount:" + wxcheckKey, keyCountValue, dt);
                                result = "{\"Status\":\"110\",\"Msg\":\"充值成功,流量为:" + keyCountValue + ";有效期:" + endDateTime + "!\"}";
                            }
                            else
                            {
                                if (RedisCacheToolsYUN0.Exists("keyCount:" + wxcheckKey))
                                {
                                    RedisCacheToolsYUN0.Remove("keyCount:" + wxcheckKey);
                                }
                                result = "{\"Status\":\"003\",\"Msg\":\"充值失败,此key不存在!\"}";
                            }
                            break;
                        case "Delete":

                            wxcheckKey = "wxcheck:" + dic[DOMAIN_KEY];

                            if (!RedisCacheToolsYUN0.Exists(wxcheckKey))
                            {
                                result = "{\"Status\":\"003\",\"Msg\":\"key不存在,联系管理员!\"}";
                            }
                            else
                            {

                                DateTime dt1 = DateTime.Parse(dic[AUTH_END_TIME]);
                                DateTime dt2 = DateTime.Now;
                                if (DateTime.Compare(dt1, dt2) < 0 || !RedisCacheToolsYUN0.Exists(wxcheckKey))
                                {
                                    if (RedisCacheToolsYUN0.Exists(wxcheckKey))
                                    {
                                        RedisCacheToolsYUN0.Remove(wxcheckKey);
                                    }

                                    if (RedisCacheToolsYUN0.Exists("keyCount:" + wxcheckKey))
                                    {
                                        RedisCacheToolsYUN0.Remove("keyCount:" + wxcheckKey);
                                    }

                                   // VoiceServerClass.DeleteSiteAuth(sae.Id.ToString());
                                    result = "{\"Status\":\"101\",\"Msg\":\"授权key删除成功!\"}";
                                }
                                else
                                {
                                    result = "{\"Status\":\"004\",\"Msg\":\"授权key未过期,不能删除!\"}";
                                }
                            }



                            break;
                        default:
                            result = "{\"Status\":\"009\",\"Msg\":\"参数错误,联系管理员!\"}";
                            break;
                    }
                    #endregion
                }
                catch (Exception e)
                {
                    result = "{\"Status\":\"010\",\"Msg\":\"" + e.Message + "\"}";
                }
            }
            #region 其它请求
            else
            {
                result = "{\"Status\":\"009\",\"Msg\":\"参数错误,联系管理员!\"}";
            }

            #endregion

            JsonpResult(context, result);


        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        protected void JsonpResult(HttpContext context, string result)
        {
            if (!string.IsNullOrEmpty(context.Request.QueryString["callback"]))
            {
                string callBack = context.Request.QueryString["callback"].ToString(); //回调

                result = callBack + "(" + result + ")";
            }
            context.Response.Write(result);

        }
    }
}