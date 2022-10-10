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
using System.Threading.Tasks;

namespace HangfireService.tasks
{
    public class WechatMomentDataJob
    {

        /// <summary>
        /// 朋友圈推送数据同步
        /// </summary>
        public  void ExecTaskList()
        {
            try
            {
                getWechatMomentList();

                Thread.SpinWait(1000);
                getWechatMomentMembers();

                Thread.SpinWait(10000);
                getWechatMomentUsers();

                Thread.SpinWait(10000);
                getWechatMomentResult();
            }
            catch (Exception ex)
            {

                throw ex;
            }

        }

        //获取企微发送朋友圈列表数据，并插入数据库
        public void getWechatMomentList()
        {
            string querySql = "SELECT DISTINCT momentid FROM wechat_momentList";
            DataTable dataTable = DBHelper.ExecuteDataTable(querySql,out string err);
            List<string> momentIds = (from d in dataTable.AsEnumerable() select d.Field<string>("momentid")).ToList();


            string token = HttpHelper.GetToken("p4qEGWQ50eX_-MbJiwX3DpFLzHqbPCTa7e1-LUkPPjM");
            string url =$@"https://qyapi.weixin.qq.com/cgi-bin/externalcontact/get_moment_list?access_token={token}";

            long begin = TimeFormat.ToUnixTimeStamp(DateTime.Now.AddDays(-1));
            long end = TimeFormat.ToUnixTimeStamp(DateTime.Now);




            WechatMomentListRequest  wechatMomentListRequest=new WechatMomentListRequest ();
            wechatMomentListRequest.StartTime= begin;
            wechatMomentListRequest.EndTime= end;

            List<WechatMomentListResult> wechatMomentListResponse = new List<WechatMomentListResult>();
            getWechatMomentListRound(url, wechatMomentListRequest, wechatMomentListResponse);

           
            string values = string.Empty;
            foreach (WechatMomentListResult result in wechatMomentListResponse)
            {
                //遍历朋友圈关键信息
                foreach (var item in result.MomentList)
                {
                    string commentName = string.Empty;
                    Text text = item.Text;
                    if (text!=null&&!string.IsNullOrEmpty(text.Content))
                    {
                        commentName=text.Content;
                    }
                    else
                    {
                        if (item.Link!=null)
                        {
                            commentName = item.Link.Title;
                        }
                    }

                    string createtime = TimeFormat.TimeStampToDateTime(Convert.ToInt64(item.CreateTime));
                    if (!momentIds.Contains(item.MomentId))
                    {
                        values += $@"('{item.MomentId}','{commentName}','{item.Creator}','{createtime}','{item.CreateType}','{item.VisibleType}'),";
                    }
                    
                }
            }
            values = values.TrimEnd(',');

            //写入数据库
            if (!string.IsNullOrEmpty(values))
            {
                string sql = $@"INSERT INTO wechat_momentList(momentid,momentitle,creator,createtime,create_type,visible_type)
SELECT momentid,momentitle,creator,createtime,create_type,visible_type FROM (VALUES {values}) 
AS t(momentid,momentitle,creator,createtime,create_type,visible_type)";

                DBHelper.ExecuteNonQuery(sql);
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


        /// <summary>
        /// 获取企微朋友圈 成员数据
        /// </summary>
        public void getWechatMomentMembers()
        {
            //查询所有企业已发送朋友圈的id
            string sql = "select  distinct momentid from [dbo].[wechat_momentList] where  create_type=0 AND createtime BETWEEN  DATEADD(mm, -1, GETDATE())  AND  GETDATE()";
            var dt= DBHelper.ExecuteDataTable(sql,out string err);
            List<string> momentids = new List<string>();
            foreach (DataRow item in dt.Rows)
            {
                momentids.Add(item["momentid"].ToString());
            }


            string token = HttpHelper.GetToken("p4qEGWQ50eX_-MbJiwX3DpFLzHqbPCTa7e1-LUkPPjM");
            string url = $@"https://qyapi.weixin.qq.com/cgi-bin/externalcontact/get_moment_task?access_token={token}";

            Dictionary<string, List<WechatMomentUserResult>> wechatMomentUserList = new Dictionary<string, List<WechatMomentUserResult>>(); 
            foreach (var item in momentids)
            {
                WechatMomentUserRequest wechatMomentUserRequest = new WechatMomentUserRequest();
                wechatMomentUserRequest.Limit = 1000;
                wechatMomentUserRequest.MomentId = item;

                wechatMomentUserList.Add(item, getWechatMomentMembersRound(url, wechatMomentUserRequest, new List<WechatMomentUserResult>()));
            }

            string values = string.Empty;
            List<string> rangUser = GetMemberRange();
            foreach (var item in wechatMomentUserList)
            {
                foreach (WechatMomentUserResult userResult in item.Value)
                {
                    foreach (var tasklst in userResult.TaskList)
                    {
                        if (rangUser.Contains(tasklst.Userid))
                        {
                            values += $@" ('{item.Key}','{tasklst.Userid}','{tasklst.PublishStatus}'),";
                        }
                        
                    }
                   
                }
            }
            if (!string.IsNullOrEmpty(values))
            {
                values = values.TrimEnd(',');
                sql = $@"
INSERT INTO wechat_moment_user(momentid,member,sendstatus)
SELECT momentid,member,sendstatus FROM (VALUES {values}) 
AS t(momentid,member,sendstatus)";
               
                ArrayList arrayList = new ArrayList();
                string preSql = @"
DELETE  FROM wechat_moment_user 
WHERE momentid IN 
(select  distinct momentid from [dbo].[wechat_momentList] where  create_type=0 AND createtime BETWEEN  DATEADD(mm, -1, GETDATE())  AND  GETDATE())";
                arrayList.Add(preSql);
                arrayList.Add(sql);
                DBHelper.ExecuteTransation(arrayList);
            }
            


            
        }


        public List<WechatMomentUserResult> getWechatMomentMembersRound(string url,WechatMomentUserRequest wechatMomentUserRequest,List<WechatMomentUserResult> wechatMomentUserResults)
        {
            var content = JsonConvert.SerializeObject(wechatMomentUserRequest);
            string result = HttpHelper.Post(content, url);

            WechatMomentUserResult wechatMomentUserResult = JsonConvert.DeserializeObject<WechatMomentUserResult>(result);
            wechatMomentUserResults.Add(wechatMomentUserResult);

            if (!string.IsNullOrEmpty(wechatMomentUserResult.NextCursor))
            {
                wechatMomentUserRequest.Cursor = wechatMomentUserResult.NextCursor;
                getWechatMomentMembersRound(url, wechatMomentUserRequest, wechatMomentUserResults);
            }
           
            return wechatMomentUserResults;
        }


        /// <summary>
        /// 获取企微朋友圈 成员发送可见客户数据
        /// </summary>
        public void getWechatMomentUsers()
        {

            //查询成员任务 对应的朋友圈推文id
            string sql = @$"select distinct  momentid,member from wechat_moment_user 
WHERE momentid IN 
(select  distinct momentid from [dbo].[wechat_momentList] where  create_type=0 AND createtime BETWEEN  DATEADD(mm, -1, GETDATE())  AND  GETDATE())
";
            var dt = DBHelper.ExecuteDataTable(sql, out string err);

            string momentid = string.Empty;

            List<MomentMember> userIds = new List<MomentMember>();
            foreach (DataRow dataRow in dt.Rows)
            {
                MomentMember mm = new MomentMember();
                mm.momentId = dataRow["momentid"].ToString();
                mm.memberId = dataRow["member"].ToString();
                userIds.Add(mm);
            }

            string token = HttpHelper.GetToken("p4qEGWQ50eX_-MbJiwX3DpFLzHqbPCTa7e1-LUkPPjM");
            string url = $@"https://qyapi.weixin.qq.com/cgi-bin/externalcontact/get_moment_customer_list?access_token={token}";


            List<System.Threading.Tasks.Task> tasks = new List<System.Threading.Tasks.Task>();


            ConcurrentDictionary<string, List<WechatMomentCustomerResult>> resultdic = new ConcurrentDictionary<string, List<WechatMomentCustomerResult>>();

            //Dictionary<string, List<WechatMomentCustomerResult>> resultdic = new Dictionary<string, List<WechatMomentCustomerResult>>();
            List<MomentMember>tempmenbers = new List<MomentMember>();

            int index = 0;
            foreach (MomentMember member in userIds)
            {
                tempmenbers.Add(member);
                if (tempmenbers.Count == 300 || index == userIds.Count - 1)
                {
                    var temp = tempmenbers;

                    tasks.Add(System.Threading.Tasks.Task.Run(() =>
                    {
                        foreach (var mm in temp)
                        {
                            WechatMomentCustomerRequest request = new WechatMomentCustomerRequest();
                            request.Userid = mm.memberId;
                            request.MomentId = mm.momentId;
                            request.Limit = 5000;

                            resultdic.TryAdd(mm.momentId + "|" + mm.memberId, getWechatMomentUsersRound(url, request, new List<WechatMomentCustomerResult>()));

                        }
                    }));

                    tempmenbers = new List<MomentMember>();
                }

                 index++;
            }



            System.Threading.Tasks.Task.WaitAll(tasks.ToArray(), -1);


            List<string> values = new List<string>();
            string value = string.Empty;
            index = 0;
            foreach (var item in resultdic)
            {
                foreach (var itemlst in item.Value)
                {
                    foreach (var customer in itemlst.CustomerList)
                    {
                        index++;
                        string[] arry = item.Key.Split('|');
                        string mid = arry[0];
                        value += $@" ('{mid}','{customer.Userid}','{customer.ExternalUserid}','1'),";
                        if (index==10000)
                        {
                            sql = $@"
INSERT INTO wechat_moment_customer(momentid,memberid,customerid,sendstatus)
SELECT momentid,memberid,customerid,sendstatus FROM (VALUES {value.TrimEnd(',')})
AS T(momentid,memberid,customerid,sendstatus)";
                            values.Add(sql);
                            index = 0;
                            value = string.Empty;
                        }
                       
                    }
                }
            }

            if (index>0)
            {
                sql = $@"
INSERT INTO wechat_moment_customer(momentid,memberid,customerid,sendstatus)
SELECT momentid,memberid,customerid,sendstatus FROM (VALUES {value.TrimEnd(',')})
AS T(momentid,memberid,customerid,sendstatus)";
                values.Add(sql);
            }
            
            if (values.Count>0)
            {
                ArrayList arrayList = new ArrayList();
                string preSql = $@"DELETE FROM  wechat_moment_customer 
WHERE momentid IN (select  distinct momentid from [dbo].[wechat_momentList] where  create_type=0 AND createtime BETWEEN  DATEADD(mm, -1, GETDATE())  AND  GETDATE())";
                arrayList.Add(preSql);
                arrayList.AddRange(values);
                arrayList.Add(sql);
                DBHelper.ExecuteTransation(arrayList);
            }



        }

        public List<WechatMomentCustomerResult> getWechatMomentUsersRound(string url, WechatMomentCustomerRequest wechatMomentCustomerRequest, List<WechatMomentCustomerResult> customerResults)
        {
            string content = JsonConvert.SerializeObject(wechatMomentCustomerRequest);
            string result = HttpHelper.Post(content, url);
            WechatMomentCustomerResult customerResult = JsonConvert.DeserializeObject<WechatMomentCustomerResult>(result);

            customerResults.Add(customerResult);
            if (!string.IsNullOrEmpty(customerResult.NextCursor))
            {
                wechatMomentCustomerRequest.Cursor = customerResult.NextCursor;
                getWechatMomentUsersRound(url, wechatMomentCustomerRequest, customerResults);
            }

            return customerResults;
        }

        /// <summary>
        /// 获取企微朋友圈 成员发送可见客户数据
        /// </summary>
        public void getWechatMomentResult()
        {

            //查询成员任务 对应的朋友圈推文id
            string sql = $@"select distinct  momentid,member from wechat_moment_user 
WHERE momentid IN 
(select  distinct momentid from [dbo].[wechat_momentList] where  create_type=0 AND createtime BETWEEN  DATEADD(mm, -1, GETDATE()) AND  GETDATE())";
            var dt = DBHelper.ExecuteDataTable(sql, out string err);

           
            List<MomentMember> userIds = new List<MomentMember>();
            foreach (DataRow dataRow in dt.Rows)
            {
                MomentMember mm = new MomentMember();
                mm.momentId = dataRow["momentid"].ToString();
                mm.memberId = dataRow["member"].ToString();
                userIds.Add(mm);
            }


            string token = HttpHelper.GetToken("p4qEGWQ50eX_-MbJiwX3DpFLzHqbPCTa7e1-LUkPPjM");
            string url = $@"https://qyapi.weixin.qq.com/cgi-bin/externalcontact/get_moment_send_result?access_token={token}";

         
           
            //Dictionary<MomentMember, List<WechatMomentCustomerResult>> resultdic = new Dictionary<MomentMember, List<WechatMomentCustomerResult>>();
            ConcurrentDictionary<MomentMember, List<WechatMomentCustomerResult>> resultdic = new ConcurrentDictionary<MomentMember, List<WechatMomentCustomerResult>>();

            List<System.Threading.Tasks.Task> tasks = new List<System.Threading.Tasks.Task>();

            List<MomentMember> tempmomentMembers = new List<MomentMember>();

            int index = 0;
            foreach (MomentMember item in userIds)
            {
                tempmomentMembers.Add(item);
                if (tempmomentMembers.Count == 300 || index == userIds.Count - 1)
                {
                    var newTemp = tempmomentMembers;
                    tempmomentMembers = new List<MomentMember>();
                    tasks.Add(System.Threading.Tasks.Task.Run(() =>
                    {
                        foreach (MomentMember mm in newTemp)
                        {
                            WechatMomentCustomerRequest request = new WechatMomentCustomerRequest();
                            request.Userid = mm.memberId;
                            request.MomentId = mm.momentId;
                            request.Limit = 5000;

                            //resultdic.Add(mm.momentId+"|"+mm.memberId, getWechatMomentUsersRound(url, request, new List<WechatMomentCustomerResult>()));
                            resultdic.TryAdd(mm, getWechatMomentResultRound(url, request, new List<WechatMomentCustomerResult>()));



                        }

                    }));
                }

                index++;
            }


            System.Threading.Tasks.Task.WaitAll(tasks.ToArray(), -1);


            string value = string.Empty;
            List<string> values = new List<string>();
            index = 0;
            foreach (var item in resultdic)
            {
                foreach (var itemlst in item.Value)
                {
                    foreach (var customer in itemlst.CustomerList)
                    {
                        index++;
                        value += $@" ('{item.Key.momentId}','{item.Key.memberId}','{customer.ExternalUserid}','1'),";
                        if (index==10000)
                        {
                               sql = $@"
            INSERT INTO wechat_moment_result(momentid,memberid,customerid,sendstatus)
            SELECT momentid,memberid,customerid,sendstatus FROM (VALUES {value.TrimEnd(',')})
            AS T(momentid,memberid,customerid,sendstatus)";
                            values.Add(sql);
                            index = 0;
                            value = string.Empty;
                        }
                       
                    }
                }
            }
            if (index > 0)
            {
                sql = $@"
            INSERT INTO wechat_moment_result(momentid,memberid,customerid,sendstatus)
            SELECT momentid,memberid,customerid,sendstatus FROM (VALUES {value.TrimEnd(',')})
            AS T(momentid,memberid,customerid,sendstatus)";
                values.Add(sql);
            }

            if (values.Count>0)
            {
             
                string updatesql = $@"insert into wechat_moment_record(lastupdatetime) values('{DateTime.Now.ToString("s")}')";
               ArrayList arrayList = new ArrayList();
                string preSql = $@"DELETE FROM wechat_moment_result 
WHERE momentid IN 
(select  distinct momentid from [dbo].[wechat_momentList] where  create_type=0 AND createtime BETWEEN  DATEADD(mm, -1, GETDATE())  AND  GETDATE())
";
                arrayList.Add(preSql);
                arrayList.AddRange(values);
                arrayList.Add(updatesql);
                DBHelper.ExecuteTransation(arrayList);
            }



        }

        public List<WechatMomentCustomerResult> getWechatMomentResultRound(string url, WechatMomentCustomerRequest wechatMomentCustomerRequest, List<WechatMomentCustomerResult> customerResults)
        {
            string content = JsonConvert.SerializeObject(wechatMomentCustomerRequest);
            string result = HttpHelper.Post(content, url);
            WechatMomentCustomerResult customerResult = JsonConvert.DeserializeObject<WechatMomentCustomerResult>(result);

            customerResults.Add(customerResult);
            if (!string.IsNullOrEmpty(customerResult.NextCursor))
            {
                wechatMomentCustomerRequest.Cursor = customerResult.NextCursor;
                getWechatMomentUsersRound(url, wechatMomentCustomerRequest, customerResults);
            }

            return customerResults;
        }



        public List<string> GetMemberRange()
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

    class MomentMember
    {
       public string momentId { get; set; }

       public string memberId { get; set; }
    }
}
