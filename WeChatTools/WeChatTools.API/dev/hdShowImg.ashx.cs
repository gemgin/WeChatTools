using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WeChatTools.Core;

namespace WeChatTools.API
{
    /// <summary>
    ///  微信公众号文章图片中转,突破微信反盗链限制
    /// </summary>
    public class hdShowImg : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            //防止盗链,增加自己服务器压力
            if (context.Request.UrlReferrer != null)
            {
                string refurl = context.Request.UrlReferrer.Host.ToString();
                string refhost = ConfigTool.ReadVerifyConfig("wxShowImg", "Other");
                if (refurl.Equals(refhost))
                {
                    getImg(context);
                }
                else
                {
                    context.Response.ContentType = "text/plain";
                    context.Response.Write("非法访问!");
                    
                }
            }
            else
            {
                getImg(context);
            }
            context.Response.End();
        }

        private void getImg(HttpContext context)
        {
            HttpHelper helper = new HttpHelper();
            HttpItem item = new HttpItem()
            {

                URL = context.Request["url"].ToString(),
                Referer = "",//必填参数,这里置空

                UserAgent = "Mozilla/5.0 (iPhone; CPU iPhone OS 5_1 like Mac OS X) AppleWebKit/534.46 (KHTML, like Gecko) Mobile/9B176 MicroMessenger/4.3.2",//useragent可有可无
                ResultType = ResultType.Byte

            };

            HttpResult res = helper.GetHtml(item);

            string code = "data:image/webp;base64," + Convert.ToBase64String(res.ResultByte);
            context.Response.ContentType = "image/webp";
            context.Response.OutputStream.Write(res.ResultByte, 0, res.ResultByte.Length);
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