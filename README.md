# 微信工具集
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)
[![PRs Welcome](https://img.shields.io/badge/PRs-welcome-brightgreen.svg)](https://github.com/gemgin/WeChatTools/pulls)
[![GitHub stars](https://img.shields.io/github/stars/gemgin/WeChatTools.svg?style=social&label=Stars)](https://github.com/gemgin/WeChatTools)
[![GitHub forks](https://img.shields.io/github/forks/gemgin/WeChatTools.svg?style=social&label=Fork)](https://github.com/gemgin/WeChatTools)

[交流QQ群：41977413](https://jq.qq.com/?_wv=1027&k=hkAvP9As "QQ群:41977413"),[QQ:391502069](http://wpa.qq.com/msgrd?v=3&uin=391502069&site=qq&menu=yes "QQ:391502069")

## 项目介绍
> 微信域名检测
- 实时检测域名能否在微信中直接访问的技术


## 使用
- 微信域名检测试用接口 [http://wx.rrbay.com/pro/wxUrlCheck.ashx?url=http://www.teu7.cn](http://wx.rrbay.com/pro/wxUrlCheck.ashx?url=http://www.teu7.cn "微信域名检测试用接口")
- QQ域名检测试用接口 [http://wx.rrbay.com/pro/qqUrlCheck.ashx?url=http://www.teu7.cn](http://wx.rrbay.com/pro/qqUrlCheck.ashx?url=http://www.teu7.cn "QQ域名检测试用接口")
- 抖音域名检测试用接口 [http://wx.rrbay.com/pro/dyUrlCheck.ashx?url=http://www.teu7.cn](http://wx.rrbay.com/pro/dyUrlCheck.ashx?url=http://www.teu7.cn "抖音域名检测试用接口")
- 域名备案查询接口 [http://wx.rrbay.com/pro/icpCheck.ashx?url=http://www.rrbay.com](http://wx.rrbay.com/pro/icpCheck.ashx?url=http://www.rrbay.com "域名备案查询接口")
```
 {"State":true, "Code","101","Data":"www.teu7.cn",  "Msg":"屏蔽"}
 {"State":true, "Code","102","Data":"www.wxcheckurl.com","Msg":"正常"}
 {"State":true, "Code","103","Data":"www.wxcheckurl.com","Msg":"检测结果为空,请重试!"}
 {"State":false,"Code","001","Data":"www.wxcheckurl.com","Msg":"非法访问，访问被拒绝,联系管理员qq:391502069"}
 {"State":false,"Code","002","Data":"www.wxcheckurl.com","Msg":"歇一歇,访问太快了,联系管理员qq:391502069"}
 {"State":false,"Code","003","Data":"www.wxcheckurl.com","Msg":"服务暂停,请联系管理员!"}
```
- 域名检测界面：http://h5.wxcheckurl.com/
 
### 顶尖技术，铸就稳定服务

微信域名检测接口采用基于系统服务开发，接口快速、稳定。

### 官方接口，实时结果

    微信域名检测采用官方接口，实时返回查询结果，准确率99.99%，API接口响应速度快，平均检测时间只需0.2秒

### 在线自助查询/API集成查询

    微信域名检测接口支持多域名不限次数提交查询，web端在线查询返回结果，无需安装插件或客户端。支持API查询、支持集成到自由系统或第三方系统

### 为用户定制的域名自动监测系统

   为了方便用户，可以为用户开发了一套域名自动检测自动切换系统，保证微信公众号活动正常进行。

> 积攒star=18000，微信域名检测核心代码和原理将在码云和github完全公开，如果你需要请star

## 2018-01-29 微信域名检测系统升级说明
- 数据库mongodb连接安全问题（部署不当，会造成重大安全漏洞）
- 微信域名检测key授权与微信域名检测服务分离，方便部署和后期运营
- 优化微信域名检测服务接口
- 检测入口分布式部署以及负载均衡

## 2018-08-20 微信链接推广系统升级说明
- 推广系统接收中转分离
- 中转统一采用redis
- 优化实时检测自动更换逻辑--采用每个小时检测某分钟检测
 
## 2018-08-23 微信域名检测系统升级说明
- 微信域名检测底层服务升级改造
- 采集cookie与检测服务分离
- redis与mongodb分离

## 2018-12-17 微信域名检测系统升级说明
- 微信域名监测服务上线
- 域名qq检测上线
- 同一个key可以微信检测，qq检测以及微信域名监测结果微信通知

## 2019-09-29 微信域名检测系统升级说明
- 域名备案信息查询服务上线
- 同一个key可以微信检测，qq检测以及域名备案信息查询

## 2021-04-19 域名检测系统升级说明
- 抖音域名安全检测接口上线
- 同一个key可以微信检测，qq检测,抖音域名检测以及域名备案信息查询