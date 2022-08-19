using Hangfire;
using Hangfire.SqlServer;
using HangfireService.tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HangfireService
{
    public class Startup
    {
        IConfiguration config;

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
           config = new ConfigurationBuilder()
          .Add(new JsonConfigurationSource { Path = "appsettings.json", ReloadOnChange = true })
          .Build();
           
            services.AddHangfire(x => x.UseStorage(new SqlServerStorage(
                config["ConnectionString"],
                new SqlServerStorageOptions
                {
                   UseRecommendedIsolationLevel=true, // ������뼶��Ĭ���Ƕ�ȡ���ύ��
                    QueuePollInterval = TimeSpan.FromSeconds(15),             //- ��ҵ������ѯ�����Ĭ��ֵΪ15�롣
                    JobExpirationCheckInterval = TimeSpan.FromHours(1),       //- ��ҵ���ڼ������������ڼ�¼����Ĭ��ֵΪ1Сʱ��
                    CountersAggregateInterval = TimeSpan.FromMinutes(5),      //- �ۺϼ������ļ����Ĭ��Ϊ5���ӡ�
                    PrepareSchemaIfNecessary = true,                          //- �������Ϊtrue���򴴽����ݿ��Ĭ����true��
                    DashboardJobListLimit = 50000,                            //- �Ǳ����ҵ�б����ơ�Ĭ��ֵΪ50000��
                    TransactionTimeout = TimeSpan.FromMinutes(10000)           //- ���׳�ʱ��Ĭ��Ϊ1���ӡ�
                }
                )));

          
          
            services.AddHangfireServer();
          
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
           
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.Map("/", context =>
                {
                    context.Response.Redirect("/hangfire");
                    return Task.CompletedTask;
                });
            });

            app.UseHangfireDashboard();//���ú�̨�Ǳ���

            //������ݿ���ҵ ����
            JobClass jobClass = new JobClass();
            RecurringJob.AddOrUpdate(() => jobClass.checkfailtask(), config["CronExprees"]);

            //��ȡ��΢����Ȧ�б� ����
            WechatMomentDataJob  wechatMomentDataJob= new WechatMomentDataJob();
            RecurringJob.AddOrUpdate(() => wechatMomentDataJob.ExecTaskList(), config["����Ȧ�����б�"],TimeZoneInfo.Local);

          

            //��ȡ��΢������Ϣ�б�
            WechatMsgSendDataJob msgSendDataJob = new WechatMsgSendDataJob();
            RecurringJob.AddOrUpdate(() => msgSendDataJob.ExecTaskList(), config["��Ϣ�����б�"],TimeZoneInfo.Local);

           

        }
    }
}
