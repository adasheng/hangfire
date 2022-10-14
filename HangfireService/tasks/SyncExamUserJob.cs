using HangfireService.common;
using HangfireService.model;
using Newtonsoft.Json;
using RestSharp;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Security.Policy;
using System.Xml.Linq;
using Method = RestSharp.Method;

namespace HangfireService.tasks
{
    public class SyncExamUserJob
    {

        public void ExecJobs()
        {
            CreateUser();
        }


        /// <summary>
        /// 模拟登录
        /// </summary>
        public string Login(string username, string pwd)
        {
            string url = string.Empty;
            url = "https://exam.panda-water.cn/api/public/login";
            RestClient client = new RestClient(url);

            string Cookie = string.Empty;
            Account account1 = new Account();
            account1.username = username;
            account1.password = pwd;
            RestClientHelper.Post(client, JsonConvert.SerializeObject(account1), ref Cookie);


            return Cookie;


        }


        public void CreateUser()
        {
            string cookie = Login("admin", "123456");
            string url = "https://exam.panda-water.cn/api/system/user/create";
            RestClient client = new RestClient(url);
            RestRequest request = new RestRequest("", Method.Post);



            Dictionary<string, string> tocreateUsers = GetToCreateUsers();
            foreach (var item in tocreateUsers)
            {
                request = new RestRequest("", Method.Post);
                User user = new User();
                user.password = "123456";
                user.rePassword = "123456";
                user.status = 1;
                user.roles = new System.Collections.Generic.List<string> { "1572889527415713794" };
                user.name = item.Value;
                user.username = item.Key;
                //循环同步
                ExecCreate(client, user, request, cookie);
            }
           
        }

        private void ExecCreate(RestClient client, User user, RestRequest request, string cookie)
        {
            request.AddHeader("Cookie", cookie);
            request.AddParameter("application/json", JsonConvert.SerializeObject(user), ParameterType.RequestBody);
            RestClientHelper.Post(client, request);
        }


        private Dictionary<string, string> GetToCreateUsers()
        {
            var dt = DBHelper.ExecuteMySQLDataTable(@"select  auth_account from  t_account  where `status`=1 and is_deleted=0 ", out string err);
            List<string> currentUsers = dt.AsEnumerable().Select(x => x.Field<string>("auth_account")).ToList();

            dt = DBHelper.ExecuteDataTable(@"SELECT  工号,名称 FROM  FLOW_USERS WHERE  工号 IS NOT NULL AND  是否删除=0", out err);

          
            Dictionary<string, string> toCreateUsers = new Dictionary<string, string>();
            foreach (DataRow dr  in dt.Rows)
            {
                if (!currentUsers.Contains(dr["工号"].ToString()))
                {
                    toCreateUsers.Add(dr["工号"].ToString(), dr["名称"].ToString());
                }
            }

            return toCreateUsers;
           
        }
    }

    class Account
    {
        public string username;

        public string password;
    }

    class User
    {
        /// <summary>
        /// 
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string username { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string password { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string rePassword { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string gender { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int status { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<string> roles { get; set; }
    }

}
