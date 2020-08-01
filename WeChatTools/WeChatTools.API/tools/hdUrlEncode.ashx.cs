using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace WeChatTools.API.tools
{
    /// <summary>
    /// url编码 解码
    /// </summary>
    public class hdUrlEncode : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            string result = string.Empty;
            context.Response.ContentType = "text/plain";
            if (context.Request.HttpMethod.ToUpper().Equals("POST"))
            {

                #region 获取post中的参数
                string model = context.Request.Form["model"]; //方法：编码 全编码 解码
                string type = context.Request.Form["type"];//编码类型  UTF8 gb2312
                string content = context.Request.Form["content"];
                #endregion
                try
                {
                    switch (model)
                    {
                        case "ue":
                            result = UrlEncode(type, content);
                            break;
                        case "uae":
                            result = UrlALLEncode(type, content);
                            break;
                        case "ud":
                        default:
                            result = UrlDecode(type, content);
                            break;


                    }
                     
                }
                catch (Exception ex)
                {
                    result = "非法访问,联系管理员!";
                }
            }
            else
            {
                result = "参数错误,联系管理员!";
            }
            context.Response.Headers.Add("Access-Control-Allow-Origin", "https://www.rrbay.com");
            context.Response.Headers.Add("Access-Control-Allow-Methods", "POST");
            context.Response.Headers.Add("Access-Control-Allow-Credentials", "true");

            context.Response.Write(result);
            context.Response.End();
        }

        //普通的编码
        protected string UrlEncode(string type, string content)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(System.Web.HttpUtility.UrlEncode(content, Encoding.GetEncoding(type)));
            return builder.ToString();

        }

        //全部编码
        protected string UrlALLEncode(string type, string content)
        {
            StringBuilder builder = new StringBuilder();
            foreach (char c in content)
            {
                if (System.Web.HttpUtility.UrlEncode(c.ToString()).Length > 1)
                {
                    builder.Append(System.Web.HttpUtility.UrlEncode(c.ToString(), Encoding.GetEncoding(type)).ToUpper());
                }
                else
                {
                    int asc = (int)c;
                    builder.Append("%" + asc.ToString("X2"));
                }
            }
            return builder.ToString();



        }

        //解码
        protected string UrlDecode(string type, string content)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(System.Web.HttpUtility.UrlDecode(content, Encoding.GetEncoding(type)));
            return builder.ToString();

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