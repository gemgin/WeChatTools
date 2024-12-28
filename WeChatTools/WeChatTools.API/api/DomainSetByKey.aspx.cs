using WeChatTools.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WeChatTools.API
{
    /// <summary>
    ///专门监控域名检测接口api
    /// </summary>
    public partial class DomainSetByKey : System.Web.UI.Page
    {


        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string key = Request.QueryString["key"];//key 加密的  

                string typekey = Request.QueryString["typekey"];//typekey 加密的  
                if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(typekey))
                {
                    try
                    {
                        TextBox1.Text = key;
                        if (RedisCacheToolsYUN0.Exists("wxcheck:" + key))
                        {
                            if (RedisCacheToolsYUN1.Exists(key))
                            {
                                string keyValue = RedisCacheToolsYUN1.Get<string>(key);
                                string[] temp = keyValue.Split(';');

                                if (temp.Length == 2 && !string.IsNullOrEmpty(temp[1]))
                                {

                                    TextBox2.Text = temp[1];
                                }
                                else
                                {
                                    TextBox2.Text = "";
                                }
                            }

                            string keyleft = "0天0时0分";
                            TimeSpan? ts = RedisCacheToolsYUN0.GetKeyTimeToLive("wxcheck:" + key);
                            keyleft = ts.Value.Days + "天" + ts.Value.Hours + "时" + ts.Value.Minutes + "分";

                            ///  keyLogs = "/Logs/" + key+".txt";
                            string domainRed = "没有屏蔽的域名";
                            if (RedisCacheToolsYUN1.Exists("Red" + key))
                            {
                                domainRed = RedisCacheToolsYUN1.Get<string>("Red" + key);
                            }
                            Label1.Text = domainRed;
                            Label2.Text = keyleft;


                        }
                        string keycount = "keyCount:wxcheck:" + key;
                        if (typekey == "0" || typekey == "-9")
                        {
                            syCount.Style.Add("display", "block");
                            if (typekey == "0")
                            {
                                typeKey.Style.Add("display", "none");
                            }
                            if (RedisCacheToolsYUN0.Exists(keycount)) {                              
                                string keycountValue = "0";
                                keycountValue = RedisCacheToolsYUN0.Get<string>(keycount);
                                Label3.Text = keycountValue;
                            }
                          
                            
                        }
                        else
                        {
                            typeKey.Style.Add("display", "block");                            
                            syCount.Style.Add("display", "none");                           
                        }

                    }

                    catch (Exception ex)
                    {
                        base.Response.Write("非法访问了:" + ex.Message);
                        base.Response.End();
                        // HttpContext.Current.Response.Write("非法访问了");

                        // throw new Exception("非法访问了");
                    }
                }
                else
                {
                    typeKey.Style.Add("display", "none");
                    syCount.Style.Add("display", "none");
                }
            }
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            string key = this.TextBox1.Text;
            string domainTemp = this.TextBox2.Text.Trim();
            domainTemp = domainTemp.Replace("，", ",").Replace(" ", "");
            if (domainTemp.Length > 1 && domainTemp.Length < 200)
            {
                if (RedisCacheToolsYUN1.Exists(key))
                {
                    TimeSpan? ts = RedisCacheToolsYUN1.GetKeyTimeToLive(key);
                    string keyValue = RedisCacheToolsYUN1.Get<string>(key);
                    string[] temp = keyValue.Split(';');
                    string nKeyValue = temp[0] + ";" + domainTemp;

                    RedisCacheToolsYUN1.Add(key, nKeyValue, TimeSpan.Parse(ts.ToString()));
                    ScriptManager.RegisterStartupScript((Page)this, base.GetType(), "msg", "alert('更新监控域名成功');window.returnValue = 'true';", true);
                }
                else
                {
                    if (RedisCacheToolsYUN0.Exists("wxcheck:" + key))
                    {

                        string keyValue = RedisCacheToolsYUN0.Get<string>("wxcheck:" + key);
                        TimeSpan? ts = RedisCacheToolsYUN0.GetKeyTimeToLive("wxcheck:" + key);
                        string[] temp = keyValue.Split(';');

                        if (temp[1] != "0")
                        {//-9 1 2 20
                            string nKeyValue = temp[0] + ";" + domainTemp;

                            RedisCacheToolsYUN1.Add(key, nKeyValue, TimeSpan.Parse(ts.ToString()));
                            ScriptManager.RegisterStartupScript((Page)this, base.GetType(), "msg", "alert('更新监控域名成功');window.returnValue = 'true';", true);
                        }
                        else
                        {// 0
                            ScriptManager.RegisterStartupScript((Page)this, base.GetType(), "msg", "alert('更新监控域名失败,流量包key不支持!');window.returnValue = 'true';", true);
                        }
                    }
                    else
                    {
                        if (RedisCacheToolsYUN1.Exists(key))
                        {
                            RedisCacheToolsYUN1.Remove(key);
                        }
                        ScriptManager.RegisterStartupScript((Page)this, base.GetType(), "msg", "alert('更新监控域名失败,key没有开通');window.returnValue = 'true';", true);
                    }
                }
            }
            else
            {
                if (RedisCacheToolsYUN1.Exists(key))
                {
                    RedisCacheToolsYUN1.Remove(key);
                }
                ScriptManager.RegisterStartupScript((Page)this, base.GetType(), "msg", "alert('更新监控域名失败,字符长度不能超过200');window.returnValue = 'true';", true);

            }
        }



    }
}