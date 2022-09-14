using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
namespace HangfireService.common
{
    public static class DBHelper
    {

        static IConfiguration configuration;
        public static string connString;

        static DBHelper()
        {
            configuration = new ConfigurationBuilder()
              .Add(new JsonConfigurationSource { Path = "appsettings.json", ReloadOnChange = true })
              .Build();
            connString = configuration["ConnectionString"].ToString();

        }
        /// <summary>
        /// 执行SQL语句，返回DataTable数据
        /// </summary>
        public static DataTable ExecuteDataTable(string sql, out string err)
        {

            err = "";

            try
            {
                DataTable dt = new DataTable();

                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();
                    using (SqlCommand command = new SqlCommand(sql, conn))
                    {
                        command.CommandTimeout = 180;
                        using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                        {
                            adapter.Fill(dt);
                        }
                    }
                    conn.Close();
                }

                return dt;
            }
            catch (Exception e)
            {
                string functionName = "?";

                try
                {
                    System.Diagnostics.StackTrace st = new System.Diagnostics.StackTrace();

                    functionName = st.GetFrame(1).GetMethod().Name;
                }
                catch { }

                err = "ERROR | 调用函数 : " + functionName + " | 错误信息 : " + e.Message;

                //Logger.Append("ExecuteDataTable", "CityServer.Lawer", 0, "error", err + " | SQL语句 : " + sql + " | 连接字符串 : " + connString, DBTool.GetClientIP());

                return new DataTable();
            }
        }

        /// <summary>
        /// 执行语句 不返回数据11
        /// </summary>
        /// <param name="sql"></param>
        public static void ExecuteNonQuery(string sql)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                using (SqlCommand command = new SqlCommand(sql, conn))
                {
                    command.CommandTimeout = 180;
                    command.ExecuteNonQuery();
                }
                conn.Close();
            }

        }



        /// <summary> 
        /// 执行事务，多SQL语句 
        /// </summary> 
        /// <param name="sqlList"></param> 
        /// <returns></returns> 
        public static bool ExecuteTransation(ArrayList sqlList)
        {

            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();
                IDbCommand cmd = conn.CreateCommand();
                cmd.Connection = conn;

                IDbTransaction tx = conn.BeginTransaction();
                cmd.Transaction = tx;

                try
                {
                    for (int i = 0; i < sqlList.Count; i++)
                    {
                        if (sqlList[i] != null)
                        {
                            string sql = sqlList[i].ToString();

                            if (sql.Trim().Length > 1)
                            {
                               
                                cmd.CommandText = sql;

                                cmd.ExecuteNonQuery();
                            }
                        }
                    }

                    tx.Commit();
                }
                catch (Exception e)
                {
                    tx.Rollback();

                    return false;
                }
            }

            return true;
        }
    }
}
