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



            List<UserInfo> tocreateUsers = GetToCreateUsers();
            foreach (var item in tocreateUsers)
            {
                request = new RestRequest("", Method.Post);
                User user = new User();
                user.password = item.no + (string.IsNullOrEmpty(item.cardNo) ? "123456" : item.cardNo.Substring(item.cardNo.Length - 6, 6));
                user.rePassword = user.password;
                user.status = 1;
                user.roles = new System.Collections.Generic.List<string> { "1572889527415713794" };
                user.name = item.name;
                user.username = item.no;
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


        private List<UserInfo> GetToCreateUsers()
        {
            var dt = DBHelper.ExecuteMySQLDataTable(@"select  auth_account from  t_account  where `status`=1 and is_deleted=0 ", out string err);
            List<string> currentUsers = dt.AsEnumerable().Select(x => x.Field<string>("auth_account")).ToList();

            dt = DBHelper.ExecuteDataTable(@"	   SELECT  工号,名称,HR.id_card AS 身份证号 FROM  FLOW_USERS u
		   LEFT JOIN tb_hr_employee HR ON HR.emp_id=u.工号
		   WHERE  工号 IS NOT NULL AND  u.是否删除=0", out err);

            List<UserInfo> users = new List<UserInfo>();

            foreach (DataRow dr in dt.Rows)
            {
                if (!currentUsers.Contains(dr["工号"].ToString()))
                {
                    UserInfo user = new UserInfo();
                    user.name = dr["名称"].ToString();
                    user.no = dr["工号"].ToString();
                    user.cardNo = dr["身份证号"].ToString();
                    users.Add(user);

                }
            }

            return users;

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

    class UserInfo
    {
        public string name { get; set; }

        public string no { get; set; }

        public string cardNo { get; set; }
    }
}
