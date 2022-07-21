using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using System.Data.SqlClient;
using System.Data;
using System;

namespace HangfireService
{
    public class JobClass
    {
        IConfiguration configuration;
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
                    markdown.content = $@"***有新的定时任务执行失败通知***
>
>
>【任务类型】：数据库作业
>【任务名称】：{item["sql_server_agent_job_name"]}
>【执行时间】：{DateTime.Now.ToString("F")}
>【失败原因】：{item["job_step_failure_message"]}
>
>
>请及时进行查看处理。 
";
                    msgBody.markdown = markdown;
                    msgBody.msgtype = "markdown";
                    msgBody.touser =configuration["touser"];
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
                    markdown.content = $@"***有新的定时任务执行失败通知***
>
>
>【任务类型】：数据库作业
>【任务名称】：{item["Key"]}
>【执行时间】：{DateTime.Now.ToString("F")}
>【失败原因】：{item["Value"]}
>
>
>请及时进行查看处理。 
";
                    msgBody.markdown = markdown;
                    msgBody.msgtype = "markdown";
                    msgBody.touser = "78432";
                    HttpHelper httpHelper = new HttpHelper();
                    httpHelper.PostMessage01(msgBody);


                }
            }
            //end

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

}
