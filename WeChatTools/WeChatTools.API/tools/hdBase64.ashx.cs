using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using WeChatTools.Core;

namespace WeChatTools.API.tools
{
    /// <summary>
    /// url编码 解码
    /// </summary>
    public class hdBase64 : IHttpHandler
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
                            result = ToolsBase64.EncodeBase64(Encoding.GetEncoding(type), content);
                            break;
                        case "ud":
                        default:
                            result = ToolsBase64.DecodeBase64(Encoding.GetEncoding(type), content);
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

        
        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}