<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DomainSetByKey.aspx.cs" Inherits="WeChatTools.API.DomainSetByKey" %>

<!DOCTYPE html>
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <meta http-equiv="Content-Type" content="text/html; charset=utf-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1, maximum-scale=1" />
    <style type="text/css">
        html, body {
            margin: 0;
            padding: 0;
        }

        h1 {
            font-size: 1.5em;
            font-weight: 600;
            line-height: 1.2;
        }

        h2 {
            margin: 2em 0 0.8em;
            font-size: 1.125em;
            line-height: 1.1;
            font-weight: 600;
        }

        ul {
            list-style-type: square;
        }

        .container {
            -webkit-font-smoothing: antialiased;
            max-width: 860px;
            margin: 0 auto;
            padding: 0 20px;
            font: normal 400 16px/1.42 'Avenir Next',Avenir,'Helvetica Neue',Helvetica,'Lantinghei SC','Hiragino Sans GB',sans-serif;
        }

        .head {
            padding: 30px 0 0;
        }

            .head h1 {
                margin: 0;
                font-size: 32px;
                color: #000;
            }

            .head h2 {
                margin: 0;
                font-size: 24px;
                font-weight: 400;
                color: rgba(0, 0, 0, 0.6);
            }

        .nav {
            padding: 10px 0 5px;
        }

            .nav a {
                font-size: 14px;
                color: #999;
                text-decoration: none;
            }

                .nav a:hover {
                    text-decoration: underline;
                }

                .nav a + a {
                    margin-left: 20px;
                }

        h3.end {
            font-weight: 400;
            margin: 20px 0 0;
            color: #ccc;
        }

        .content {
            margin-left: -20px;
            margin-right: -20px;
        }

        .item {
            display: inline-block;
            padding: 20px;
            text-align: center;
        }

            .item a {
                display: inline-block;
                color: #333;
                text-decoration: none;
                font-size: 14px;
            }

            .item img {
                width: 60px;
                height: 60px;
                border-radius: 10%;
            }

            .item strong {
                display: block;
                font-weight: 600;
            }

        .copy {
            padding: 20px 0;
            font-size: 14px;
            color: #ccc;
        }

        .yue a {
            color: #db4d6d;
        }

        @media (max-width:680px) {
            .nav a + a {
                margin-left: 10px;
            }
        }
    </style>
    <title>更新监控域名</title>
</head>
<body>
    <form id="form1" runat="server">
        <div class="container">
            <br />
            <div class="yue">
                <asp:TextBox ID="TextBox1" runat="server" ReadOnly="True" Height="30px" Width="650px"></asp:TextBox>
            </div>
            <br />
            <div class="yue">
                Key剩余时间: <strong style="color: #FF0000">
                    <asp:Label ID="Label2" runat="server" Text="0天0时0分"></asp:Label></strong>
            </div>
             <br />
            <div id="typeKey" name="typeKey" runat="server">
                <p>添加好的监测域名，每天进入检测队列排队检测，大概5分钟左右反馈结果以QQ邮件通知您。</p>
                <br />
                <p>下面是剩余的监控域名,监控域名之间以<strong style="color: #FF0000">英文逗号</strong>分开,例如: <strong style="color: #FF0000">aa.com,www.bb.com,wap.cc.com</strong></p>
                <br />
                <div class="yue">
                    <asp:TextBox ID="TextBox2" runat="server" Height="30px" Width="650px"></asp:TextBox>
                </div>
                <br />
                <div class="yue">
                    <asp:Button ID="Button1" runat="server" OnClick="Button1_Click" Text="更新监控域名" Height="40px" Width="125px" />
                </div>
                <br />
                <div>
                    屏蔽的监控域名: <strong style="color: #FF0000">
                        <asp:Label ID="Label1" Style="word-break: break-all; word-wrap: break-word" runat="server" Text="没有屏蔽的域名"></asp:Label></strong>
                </div>               
            </div>
             <br />
            <div class="yue" id="syCount" name="syCount" runat="server">
                <div>
                    剩余流量次数: <strong style="color: #FF0000">
                        <asp:Label ID="Label3" runat="server" Text="0"></asp:Label></strong>
                </div>
            </div>
        </div>
    </form>
</body>
</html>
