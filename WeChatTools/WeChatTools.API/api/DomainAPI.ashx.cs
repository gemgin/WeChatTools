using WeChatTools.Core;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;

namespace WeChatTools.API
{
    /// <summary>
    /// 专门监控域名检测接口api
    /// </summary>
    public class DomainAPI : IHttpHandler
    {
        protected const string S_PROC_NAME = "sProcName";              //调用方法名
        protected const string DOMAIN_KEY = "domainKey";               //微信域名检测key
        protected const string DOMAIN_LIST = "domainList";         //落地域名列表
        protected const string OVER_TIME = "overTime";          //授权截至时间   
        protected const string OPENID = "openId";          //用户id
        protected const string USER_ID = "userId";                     //代理商id
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
                    #region 判断用户id是否存在操作权限
                    
                    string key = dic[DOMAIN_KEY];
                    string wxCheckApiKey = ConfigTool.ReadVerifyConfig("wxCheckId", "CheckKey");

                    if (string.IsNullOrEmpty(dic[USER_ID]) || dic[USER_ID] != wxCheckApiKey)
                    {
                        //代理商不存在
                        result = "{\"Code\":\"009\",\"Msg\":\"非法访问,联系管理员!\"}";
                        JsonpResult(context, result);
                        return;
                    }                   
                    #endregion

                    #region 获取授权key信息

                    switch (sProName)
                    {
                        case "Update":
                            if (RedisCacheToolsYUN1.Exists(key))
                            {
                                //更新
                                string domainTemp = RedisCacheToolsYUN1.Get<string>(key);
                                string keyValue = domainTemp;
                                string openid = dic[OPENID];
                                if (!keyValue.Contains(";"))
                                {

                                    keyValue = openid + ";" + domainTemp;
                                }
                                else
                                {
                                    string[] temp = keyValue.Split(';');
                                    keyValue = openid + ";" + temp[1];//可以更新openid
                                }
                                RedisCacheToolsYUN1.Add(key, keyValue, DateTime.Parse(dic[OVER_TIME]));

                            }
                            result = "{\"Code\":\"101\",\"Msg\":\"续期时间成功!\"}";
                            break;
                        case "Delete":
                            if (RedisCacheToolsYUN1.Exists(key))
                            {
                                RedisCacheToolsYUN1.Remove(key);
                            }
                            result = "{\"Code\":\"102\",\"Msg\":\"授权key删除成功!\"}";
                            break;
                        default:
                            result = "{\"Code\":\"009\",\"Msg\":\"参数错误,非法访问1!\"}";
                            break;
                    }
                    #endregion
                }
                catch (Exception e)
                {
                    result = "{\"Code\":\"010\",\"Msg\":\"" + e.Message + "\"}";
                }
            }
            #region 其它请求
            else
            {
                result = "{\"Code\":\"009\",\"Msg\":\"参数错误,非法访问2!\"}";
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