using HangfireService.common;
using HangfireService.model;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;

namespace HangfireService.tasks
{
    public class WechatMomentDataJob
    {
        //获取企微发送朋友圈列表数据，并插入数据库
        public void getWechatMomentList()
        {
            string token = HttpHelper.GetToken();
            string url =$@"https://qyapi.weixin.qq.com/cgi-bin/externalcontact/get_moment_list?access_token={token}";

            long begin= TimeFormat.ToUnixTimeStamp(Convert.ToDateTime("2022-08-01"));
            long end = TimeFormat.ToUnixTimeStamp(Convert.ToDateTime("2022-08-09"));
            WechatMomentListRequest  wechatMomentListRequest=new WechatMomentListRequest ();
            wechatMomentListRequest.StartTime= begin;
            wechatMomentListRequest.EndTime= end;

            List<WechatMomentListResult> wechatMomentListResponse = new List<WechatMomentListResult>();
            getWechatMomentListRound(url, wechatMomentListRequest, wechatMomentListResponse);

            string sql = "";
            foreach (var result in wechatMomentListResponse)
            {
                
            }

        }

        public List<WechatMomentListResult> getWechatMomentListRound(string url, WechatMomentListRequest wechatMomentListRequest, List<WechatMomentListResult> wechatMomentListResults)
        {
            var content = JsonConvert.SerializeObject(wechatMomentListRequest);
            string result = HttpHelper.Post(content, url);

            WechatMomentListResult wechatMomentListResult = JsonConvert.DeserializeObject<WechatMomentListResult>(result);
            wechatMomentListResults.Add(wechatMomentListResult);
            if (!string.IsNullOrEmpty(wechatMomentListResult.NextCursor))
            {
                wechatMomentListRequest.Cursor = wechatMomentListResult.NextCursor;
                getWechatMomentListRound(url, wechatMomentListRequest, wechatMomentListResults);
            }
            return wechatMomentListResults;
        }


    }
}
