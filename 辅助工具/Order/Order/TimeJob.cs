using System.Configuration;
using Quartz;
using Quartz.Impl;

namespace Order
{
    public class TimeJob : IJob
    {
        public void Execute(IJobExecutionContext context)
        {
            Common.Log("作业调度方法执行!");
            Common.Log("预约接口返回:" + Common.PullOrderService());
        }
        public static void JobStat()
        {
            Common.Log("作业调度初始化完成!");
            ISchedulerFactory schedFact = new StdSchedulerFactory();
            var sched = schedFact.GetScheduler();
            sched.Start();
            var job = JobBuilder.Create<TimeJob>()
                .WithIdentity("myJob", "group")
                .Build();
            var trigger = TriggerBuilder.Create()
                .WithIdentity("myTrigger", "group")
                .StartNow()
                .WithCronSchedule(ConfigurationManager.AppSettings["cron_schedule"])
                .Build();
            sched.ScheduleJob(job, trigger);
        }
    }
}