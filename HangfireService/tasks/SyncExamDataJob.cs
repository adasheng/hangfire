using HangfireService.common;
using MySql.Data.MySqlClient;
using System.Collections;
using System.Data;
using System;
using HangfireService.model;
using static HangfireService.model.ExamModel;

namespace HangfireService.tasks
{
    public class SyncExamDataJob
    {
        public void ExecJobs()
        {

            //1.查询近某几个小时前的数据
            //2.插入数据库临时表
            //3.使用Merge Into  更新到SQL SERVER的目标表中
            //4.删除临时表
            int interval = 8;

            System.Threading.Tasks.Task.Run(() =>
            {
                //更新考试项目数据
                string sql = $@" SELECT A.id,A.`name`,A.create_at,A.create_by,C.auth_account,A.setting FROM  t_project  A 
LEFT JOIN t_account C ON C.user_id=A.create_by
WHERE A.create_at BETWEEN date_add(now(), interval - {interval} HOUR) AND NOW()";

                DataTable dt = DBHelper.ExecuteMySQLDataTable(sql, out string errMsg);


                string value = string.Empty;
                string begendate = string.Empty;
                string enddate = string.Empty;

                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow item in dt.Rows)
                    {
                       
                        ExamSettingModel settingModel= Newtonsoft.Json.JsonConvert.DeserializeObject<ExamSettingModel>(item["setting"].ToString());
                        if (settingModel.examSetting!=null)
                        {
                            begendate = TimeFormat.TimeStampToDateTime(settingModel.examSetting.startTime);
                            enddate = TimeFormat.TimeStampToDateTime(settingModel.examSetting.endTime);
                        }
                       

                        value += $@" SELECT '{item["id"]}' AS id,'{item["name"]}' AS name,'{item["create_at"]}' AS create_at,'{item["auth_account"]}' AS auth_account, '{begendate}' AS starttime, '{enddate}' AS endtime  UNION ALL";
                    }
                    string dtName = Tool.GenStr(6, false);
                    value = value.Remove(value.Length - 10);//删除末尾union all
                    string insertSql = $@" SELECT  * INTO #{dtName} FROM ({value})T";
                    DBHelper.ExecuteNonQuery(insertSql);

                    string mergSql = $@"merge into exam_project A using #{dtName} B on A.id=B.id
                                       when matched then update set
                                       A.name=B.name,
                                       A.create_at=B.create_at,
                                       A.auth_account=B.auth_account
                                       
                                       when not matched then insert (id,name,create_at,auth_account,starttime,endtime)
                                       values(
                                       B.id,
                                       B.name,
                                       B.create_at,
                                       B.auth_account,
                                       B.starttime,
                                       B.endtime
                                               );";

                    ArrayList arrayList = new ArrayList();
                    arrayList.Add(insertSql);
                    arrayList.Add(mergSql);
                    arrayList.Add($@"drop table #{dtName}");

                    DBHelper.ExecuteTransation(arrayList);

                }
            });

            System.Threading.Tasks.Task.Run(() =>
            {

                //更新考试结果数据
                string sql = $@" 
                                 SELECT A.project_id,A.id,A.exam_score,A.create_at,A.create_by,c.auth_account FROM t_answer A  
                                 INNER JOIN t_project P ON P.id=A.project_id
                                 LEFT JOIN t_account C ON C.user_id=A.create_by
                                 WHERE A.create_at BETWEEN date_add(now(), interval - {interval} HOUR) AND NOW()";

                DataTable dt = DBHelper.ExecuteMySQLDataTable(sql, out string errMsg);

                string value = string.Empty;
                if (dt.Rows.Count > 0)
                {
                    foreach (DataRow item in dt.Rows)
                    {
                        value += $@" SELECT '{item["project_id"]}' AS project_id,'{item["id"]}' AS id,'{item["exam_score"]}' AS exam_score,'{item["create_at"]}' AS create_at,'{item["auth_account"]}' AS auth_account  UNION ALL";
                    }

                    string dtName = Tool.GenStr(6, false);
                    value = value.Remove(value.Length - 10);//删除末尾union all
                    string insertSql = $@" SELECT  * INTO #{dtName} FROM ({value})T";
                    DBHelper.ExecuteNonQuery(insertSql);


                    string mergSql = $@"merge into exam_result A using #{dtName} B on A.id=B.id
                                         when matched then update set
                                         A.project_id=B.project_id,
                                         A.exam_score=B.exam_score,
                                         A.create_at=B.create_at,
                                         A.auth_account=B.auth_account

                                         when not matched then insert 
                                         values(
                                         B.id,
                                         B.project_id,
                                         B.exam_score,
                                         B.create_at,
                                         B.auth_account
                                         );";

                    ArrayList arrayList = new ArrayList();
                    arrayList.Add(insertSql);
                    arrayList.Add(mergSql);
                    arrayList.Add($@"drop table #{dtName}");

                    DBHelper.ExecuteTransation(arrayList);
                }
            });
        }
    }
}
