using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using System.Data.SqlClient;
using System.Data;
using System;
using System.IO;

namespace HangfireService
{
    public class JobClass
    {
        IConfiguration configuration;
        string msgText;
        public JobClass()
        {
            configuration = new ConfigurationBuilder()
              .Add(new JsonConfigurationSource { Path = "appsettings.json", ReloadOnChange = true })
              .Build();


        }

        public void checkfailtask()
        {

            DataTable dataTable = new DataTable();
            using (SqlConnection connection = new SqlConnection(configuration["ConnectionString"]))
            {

                connection.Open();
                string sql = "exec monitor_job_failures_forReportingService";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(command);

                    sqlDataAdapter.Fill(dataTable);

                }
            }


            if (dataTable.Rows.Count > 0)
            {
                foreach (DataRow item in dataTable.Rows)
                {
                    MsgBody msgBody = new MsgBody();
                    Markdown markdown = new Markdown();
                    string content = GetMsgContent().Replace("task", item["sql_server_agent_job_name"].ToString())
                         .Replace("date", DateTime.Now.ToString("F")).Replace("msg", item["job_step_failure_message"].ToString());
                    markdown.content = content;
                    msgBody.markdown = markdown;
                    msgBody.msgtype = "markdown";
                    msgBody.touser = configuration["touser"];
                    HttpHelper httpHelper = new HttpHelper();
                    httpHelper.PostMessage01(msgBody);


                }
            }

            //测试代码begin
            DataTable dataTable1 = new DataTable();
            using (SqlConnection connection = new SqlConnection(configuration["ConnectionString"]))
            {

                connection.Open();
                string sql = "SELECT * FROM [HangFire].[Set] WHERE [Key]='TEST'";
                using (SqlCommand command = new SqlCommand(sql, connection))
                {
                    SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(command);

                    sqlDataAdapter.Fill(dataTable1);

                }
            }


            if (dataTable1.Rows.Count > 0)
            {
                foreach (DataRow item in dataTable1.Rows)
                {
                    MsgBody msgBody = new MsgBody();
                    Markdown markdown = new Markdown();
                    string content = GetMsgContent().Replace("task", item["key"].ToString())
                        .Replace("date", DateTime.Now.ToString("F")).Replace("msg", item["value"].ToString());
                    markdown.content = content;
                    msgBody.markdown = markdown;
                    msgBody.msgtype = "markdown";
                    msgBody.touser = configuration["touser_test"];
                    HttpHelper httpHelper = new HttpHelper();
                    httpHelper.PostMessage01(msgBody);


                }
            }
            //end

        }



        public string GetMsgContent()
        {
            string text = string.Empty;
            string postPath = AppDomain.CurrentDomain.BaseDirectory + "\\msgstyle.txt";

            FileStream fs = null;
            if (File.Exists(postPath))
            {
                text = System.IO.File.ReadAllText(postPath);

            }
            else
            {
                text = "";
            }



            return text;
        }
    }
    [Serializable]
    public class Markdown
    {
        /// <summary>
        /// 
        /// </summary>
        public string content { get; set; }
    }
    [Serializable]
    public class MsgBody
    {
        
        public MsgBody()
        {
            agentid = 1000106;
        }
        public string touser { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string toparty { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string totag { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string msgtype { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int agentid { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Markdown markdown { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int enable_duplicate_check { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int duplicate_check_interval { get; set; }
    }


    [Serializable]
    public class tokenBody
    {
        /// <summary>
        /// 
        /// </summary>
        public int errcode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string errmsg { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string access_token { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int expires_in { get; set; }
    }


    [Serializable]
    public class SendMsgResult
    {
        /// <summary>
        /// 
        /// </summary>
        public int errcode { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string errmsg { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string msgid { get; set; }
    }

    

}
