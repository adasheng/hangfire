using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using System.Data.SqlClient;
using System.Data;
using System;
using System.IO;
using HangfireService.common;

namespace HangfireService.tasks
{
    public class CRMDataBaseJob
    {

        public void ExecJobs()
        {
            string sql = "EXEC Proc_CRM_ProjectViewLog";
            DBHelper.ExecuteNonQuery(sql);



        }

    }
}
