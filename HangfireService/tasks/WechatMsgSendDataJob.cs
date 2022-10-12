using HangfireService.common;
using HangfireService.model;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using static HangfireService.model.Wechat_Msg;

namespace HangfireService.tasks
{
    public class WechatMsgSendDataJob
    {
        /// <summary>
        /// 群发消息数据同步
        /// </summary>
        public void ExecTaskList()
        {
            try
            {
                WechatClient_SyncMsgInfo();
                Thread.SpinWait(1000);
                WechatClient_SyncMsgTask();
                Thread.SpinWait(10000);
                WechatClient_SyncMsgResult();
            }
            catch (Exception ex)
            {

                throw ex;
            }
           
        }

        ///// <summary>
        ///// 同步消息推送详情
        ///// </summary>
        ///// <returns></returns>
        public void WechatClient_SyncMsgInfo()
        {
           
           
            string cusor = string.Empty;

            //默认同步昨天
            DateTime endDate = DateTime.Now;
            DateTime beginDate = endDate.AddDays(-1);
      

            string token = HttpHelper.GetToken("p4qEGWQ50eX_-MbJiwX3DpFLzHqbPCTa7e1-LUkPPjM");
            string url = string.Format("https://qyapi.weixin.qq.com/cgi-bin/externalcontact/get_groupmsg_list_v2?access_token={0}", token);

            List<WeChatMassSendModelResult> sendModelResult = new List<WeChatMassSendModelResult>();
            GetMsgInfo(sendModelResult, "", url, beginDate, endDate);



            List<MassSendModel> massSends = new List<MassSendModel>();

            foreach (var Result in sendModelResult)
            {
                foreach (var item in Result.GroupMsgList)
                {
                    MassSendModel massSend = new MassSendModel();
                    massSend.MsgId = item.Msgid;
                    massSend.Creator = item.Creator;
                    massSend.CreateDate = TimeFormat.TimeStampToDateTime(Convert.ToInt64(item.CreateTime));
                    massSend.MsgType = item.CreateType.ToString();

                    foreach (var attachment in item.Attachments)
                    {

                        if (attachment.Link != null)
                        {
                            massSend.MsgName = attachment.Link.Title;
                        }
                        else
                        {
                            massSend.MsgName = item.Text is null ? "无标题" : item.Text.Content;
                        }
                    }
                    massSends.Add(massSend);
                }

            }

            //查询数据库已存在消息
            string querySql = "SELECT DISTINCT MsgId FROM wechat_MsgList";
            DataTable dataTable = DBHelper.ExecuteDataTable(querySql,out string err);
            List<string> msgIds = (from d in dataTable.AsEnumerable() select d.Field<string>("MsgId")).ToList();

            string value = string.Empty;

           
            foreach (var massSend in massSends)
            {
                if (!msgIds.Contains(massSend.MsgId))
                {
                    value += $@"('{massSend.MsgId}','{massSend.MsgName}','{massSend.CreateDate}','{massSend.Creator}','{massSend.MsgType}','{massSend.MsgOrigin}','','{DateTime.Now}'),";
                }

            }
            if (!string.IsNullOrEmpty(value))
            {


                string sql = $@" INSERT INTO wechat_MsgList([MsgId], [MsgName], [CreateDate], [Creator], [MagType], [MsgSource], [MsgDetails],[Synctime])
SELECT [MsgId], [MsgName], [CreateDate], [Creator], [MagType], [MsgSource], [MsgDetails],[Synctime] FROM (values{value.TrimEnd(',')})
AS t( [MsgId], [MsgName], [CreateDate], [Creator], [MagType], [MsgSource], [MsgDetails],[Synctime])";

                DBHelper.ExecuteNonQuery(sql);
            }
         
        }


        public void GetMsgInfo(List<WeChatMassSendModelResult> sendModelResult, string cusor, string url, DateTime beginTime, DateTime endTime)
        {

            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));//当地时区

            TimeSpan begints = beginTime - startTime;
            TimeSpan endts = endTime - startTime;

            WeChatMassSendModelRequstBody body = new WeChatMassSendModelRequstBody();
            body.StartTime = Convert.ToInt64(begints.TotalSeconds);
            body.EndTime = Convert.ToInt64(endts.TotalSeconds);
            body.ChatType = "single";

            string json = HttpHelper.Post(Newtonsoft.Json.JsonConvert.SerializeObject(body), url);
           
            WeChatMassSendModelResult weChatMassSendModelResult =JsonConvert.DeserializeObject<WeChatMassSendModelResult>(json);
            sendModelResult.Add(weChatMassSendModelResult);
            cusor = weChatMassSendModelResult.NextCursor;
            if (string.IsNullOrEmpty(cusor))
            {
                return;
            }



            GetMsgInfo(sendModelResult, cusor, url, beginTime, endTime);
        }



        /// <summary>
        /// 拉取推送任务
        /// </summary>
        /// <returns></returns>
        public void WechatClient_SyncMsgTask()
        {

            string token = HttpHelper.GetToken("p4qEGWQ50eX_-MbJiwX3DpFLzHqbPCTa7e1-LUkPPjM");
            string url = string.Format("https://qyapi.weixin.qq.com/cgi-bin/externalcontact/get_groupmsg_task?access_token={0}", token);
            List<WeChatSendTaskResult> taskResults = new List<WeChatSendTaskResult>();
            string nextCusor = string.Empty;

            string sql = "SELECT DISTINCT MsgId FROM  wechat_MsgList WHERE MagType=0  AND CreateDate Between  DATEADD(mm, -1, GETDATE())  AND  GETDATE()";
            var dt = DBHelper.ExecuteDataTable(sql, out string err);
            List<string> lstID = (from d in dt.AsEnumerable() select d.Field<string>("MsgId")).ToList();
            foreach (var item in lstID)
            {
                GetMsgTask(taskResults, url, nextCusor, item);
            }

            string values = string.Empty;
            List<string> rangeUser = GetMemberRange();
            foreach (var taskResult in taskResults)
            {
                foreach (var item in taskResult.TaskList)
                {
                    if (rangeUser.Contains(item.Userid))
                    {
                        values += $@"('{taskResult.MsgId}','{item.Userid}','{TimeFormat.TimeStampToDateTime(Convert.ToInt64(item.SendTime))}','{item.Status}'),";
                    }

                }
            }

            values = values.TrimEnd(',');

            if (!string.IsNullOrEmpty(values))
            {
                sql = $@"DELETE  FROM A FROM  wechat_MsgTaskList A INNER JOIN weChat_MsgList M ON M.MsgId=A.MstId
WHERE M.CreateDate Between  DATEADD(mm, -1, GETDATE())  AND  GETDATE() AND M.MagType=0;

INSERT INTO wechat_MsgTaskList([MstId], [MemberId], [SendTime], [SendStatus])
SELECT [MstId], [MemberId], [SendTime], [SendStatus] FROM (values {values})
AS t( [MstId], [MemberId], [SendTime], [SendStatus])";
                DBHelper.ExecuteNonQuery(sql);
            }

        }

        public void GetMsgTask(List<WeChatSendTaskResult> taskResults, string url, string nextCusor, string MsgId)
        {
            WeChatSendTaskRequstBody requstBody = new WeChatSendTaskRequstBody();
            requstBody.Msgid = MsgId;
            requstBody.Limit = 1000;
            requstBody.Cursor = nextCusor;

            string json = HttpHelper.Post(JsonConvert.SerializeObject(requstBody), url);
            WeChatSendTaskResult taskResult = JsonConvert.DeserializeObject<WeChatSendTaskResult>(json);
            taskResult.MsgId = MsgId;
            taskResults.Add(taskResult);

            nextCusor = taskResult.NextCursor;
            if (string.IsNullOrEmpty(nextCusor))
            {
                return;
            }



            GetMsgTask(taskResults, url, nextCusor, MsgId);
        }


        public void WechatClient_SyncMsgResult()
        {

            string token = HttpHelper.GetToken("p4qEGWQ50eX_-MbJiwX3DpFLzHqbPCTa7e1-LUkPPjM");
            string url = string.Format("https://qyapi.weixin.qq.com/cgi-bin/externalcontact/get_groupmsg_send_result?access_token={0}", token);

            string sql = @"SELECT DISTINCT  MstId,MemberId FROM wechat_MsgTaskList l INNER JOIN weChat_MsgList m ON M.MsgId=L.MstId
WHERE M.CreateDate Between  DATEADD(mm, -1, GETDATE())  AND  GETDATE()  AND  M.MagType=0";
            var dt = DBHelper.ExecuteDataTable(sql,out string err);

            List<System.Threading.Tasks.Task> tasks = new List<System.Threading.Tasks.Task>();
            int index = 0;
            List<DataRow> dataRows = new List<DataRow>();
            ConcurrentBag<string> sqls = new ConcurrentBag<string>();
            foreach (DataRow drr in dt.Rows)
            {
                dataRows.Add(drr);
                if (dataRows.Count == 300 || index == dt.Rows.Count - 1)
                {
                    List<DataRow> NewRows = dataRows;
                    dataRows = new List<DataRow>();
                    tasks.Add(System.Threading.Tasks.Task.Run(() =>
                    {
                        List<WeChatSendMsgResult> weChatSendMsgResults = new List<WeChatSendMsgResult>();
                        foreach (DataRow dr in NewRows)
                        {
                            GetMsgResult(weChatSendMsgResults, url, string.Empty, dr["MemberId"].ToString(), dr["MstId"].ToString());
                        }

                        string value = string.Empty;
                        foreach (var item in weChatSendMsgResults)
                        {
                            foreach (var sendList in item.SendList)
                            {
                                value += $@"('{item.MsgId}','{sendList.Userid}','{sendList.ExternalUserid}','{sendList.Status}','{TimeFormat.TimeStampToDateTime(Convert.ToInt64(sendList.SendTime))}'),";
                            }
                        }

                       string sql1 = $@" INSERT INTO [dbo].[wechat_MsgSendResult]([MsgId], [UserId], [External_userId], [SendStutas], [SendTime])
SELECT [MsgId], [UserId], [External_userId], [SendStutas], [SendTime] FROM (VALUES {value.TrimEnd(',')} )  AS T 
([MsgId], [UserId], [External_userId], [SendStutas], [SendTime]) ";
                        sqls.Add(sql1);
                        //DataBaseHelper.ExecuteNonQuery(sql);
                    }));

                }
                index++;
            }

            System.Threading.Tasks.Task.WaitAll(tasks.ToArray(), -1);

            string truncateSql = @"DELETE FROM A FROM wechat_MsgSendResult A INNER JOIN weChat_MsgList M ON M.MsgId=A.MsgId
WHERE M.CreateDate Between  DATEADD(mm, -1, GETDATE())  AND GETDATE() AND M.MagType = 0";
            ArrayList arrayList = new ArrayList();
            arrayList.Add(truncateSql);
            arrayList.AddRange(sqls.ToArray());

            DBHelper.ExecuteTransation(arrayList);
            
        }

        public void GetMsgResult(List<WeChatSendMsgResult> weChatSendMsgResults, string url, string nextCursor, string userId, string MsgId)
        {
            WeChatSendMsgResultRequestBody requstBody = new WeChatSendMsgResultRequestBody();
            requstBody.Limit = 1000;
            requstBody.Userid = userId;
            requstBody.Msgid = MsgId;

            string json = HttpHelper.Post(JsonConvert.SerializeObject(requstBody), url);

            WeChatSendMsgResult taskResult = JsonConvert.DeserializeObject<WeChatSendMsgResult>(json);
            taskResult.MsgId = MsgId;
            weChatSendMsgResults.Add(taskResult);
            nextCursor = taskResult.NextCursor;
            if (string.IsNullOrEmpty(nextCursor))
            {
                return;
            }
            GetMsgResult(weChatSendMsgResults, url, nextCursor, userId, MsgId);
        }

        public  List<string> GetMemberRange()
        {
            List<string> list = new List<string>();
            string sql = "SELECT 工号 FROM  企业微信添加总名单";
            var result = DBHelper.ExecuteDataTable(sql, out string err);
            foreach (DataRow item in result.Rows)
            {
                list.Add(item["工号"].ToString());
            }

            return list;
        }

    }
}
