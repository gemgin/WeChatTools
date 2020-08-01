using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace WeChatTools.Core
{
    public class ConfigTool
    {
        public static string xmlRootPath = System.Web.HttpRuntime.AppDomainAppPath.ToString() + "/Config";

        /// <summary>
        /// 检查配置文件是否存在
        /// </summary>
        public static bool VerifyXmlFile(string verifyName)
        {
            return System.IO.File.Exists(xmlRootPath + "/" + verifyName + ".config") == false ? false : true;
        }
        #region=============读配置==============

        /// <summary>
        /// 读取配置文件
        /// </summary>
        /// <param name="key">配置节点名称</param>
        /// <param name="cofigName">文件名</param>
        /// <returns>配置节点值</returns>
        public static string ReadVerifyConfig(string key, string verifyName)
        {
            //判断是否存在某个节点
            XmlDocument xmldoc = new XmlDocument();
            xmldoc.Load(xmlRootPath + "/" + verifyName + ".config");
            XmlNodeList xnl = xmldoc.SelectSingleNode("configuration").ChildNodes;

            string Temp = String.Empty;
            for (int i = 0; i < xnl.Count; i++)
            {
                XmlNode c = xnl[i];
                if (c.Attributes != null)
                {
                    Temp = c.Attributes["key"].Value;
                    string Value = c.Attributes["value"].Value;
                    if (Temp == key)
                    {
                        return (Value.ToString() == null || Value.ToString() == "") ? "" : Value;
                    }
                }
            }
            return "";
        }

        #endregion
        #region=============写配置==============

        /// <summary>
        /// <summary>
        /// 写配置节点
        /// </summary>
        /// <param name="key">配置节点名称</param>
        /// <param name="values">配置节点值</param>
        public static void WriteVerifyConfig(string key, string values, string verifyName)
        {
            //判断是否存在某个节点
            XmlDocument xmldoc = new XmlDocument();
            xmldoc.Load(xmlRootPath + "/" + verifyName + ".config");
            XmlNodeList xnl = xmldoc.SelectSingleNode("configuration").ChildNodes;

            string Temp = String.Empty;
            bool isExist = false;
            for (int i = 0; i < xnl.Count; i++)
            {

                XmlNode c = xnl[i];
                if (c.Attributes != null)
                {
                    Temp = c.Attributes["key"].Value;
                    if (Temp == key)
                    {
                        isExist = true;
                        c.Attributes["value"].Value = values;
                    }
                }
            }
            //如果不存在该节点,抛出异常
            if (!isExist)
            {
                throw new Exception("不存在该配置节点!");
            }
            //保存
            xmldoc.Save(xmlRootPath + "/" + verifyName + ".config");
        }
        #endregion



    }
}
