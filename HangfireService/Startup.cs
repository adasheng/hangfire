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
                   UseRecommendedIsolationLevel=true, // 事务隔离级别。默认是读取已提交。
                    QueuePollInterval = TimeSpan.FromSeconds(15),             //- 作业队列轮询间隔。默认值为15秒。
                    JobExpirationCheckInterval = TimeSpan.FromHours(1),       //- 作业到期检查间隔（管理过期记录）。默认值为1小时。
                    CountersAggregateInterval = TimeSpan.FromMinutes(5),      //- 聚合计数器的间隔。默认为5分钟。
                    PrepareSchemaIfNecessary = true,                          //- 如果设置为true，则创建数据库表。默认是true。
                    DashboardJobListLimit = 50000,                            //- 仪表板作业列表限制。默认值为50000。
                    TransactionTimeout = TimeSpan.FromMinutes(10000)           //- 交易超时。默认为1分钟。
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

            app.UseHangfireDashboard();//配置后台仪表盘

            //监控数据库作业 任务
            JobClass jobClass = new JobClass();
            RecurringJob.AddOrUpdate(() => jobClass.checkfailtask(), config["CronExprees"]);

            //拉取企微朋友圈列表 服务
            WechatMomentDataJob  wechatMomentDataJob= new WechatMomentDataJob();
            RecurringJob.AddOrUpdate(() => wechatMomentDataJob.ExecTaskList(), config["朋友圈推送列表"],TimeZoneInfo.Local);

          

            //拉取企微推送消息列表
            WechatMsgSendDataJob msgSendDataJob = new WechatMsgSendDataJob();
            RecurringJob.AddOrUpdate(() => msgSendDataJob.ExecTaskList(), config["消息推送列表"],TimeZoneInfo.Local);

           

        }
    }
}
