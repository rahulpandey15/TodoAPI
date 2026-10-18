using Hangfire;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using Todo.Application.Contracts;

namespace Todo.Infrastructure.BackgroundJobs
{
    public class HanfireBackgroundJobService : IBackgroundJobService
    {
        private readonly IBackgroundJobClient _client;
        private readonly IRecurringJobManager _recurring;

        public HanfireBackgroundJobService(
            IBackgroundJobClient client,
            IRecurringJobManager recurring)
        {
            this._client = client;
            this._recurring = recurring;
        }

        public void AddOrUpdateRecurring<T>(string jobId, Expression<Func<T, Task>> job, string cron)
        {
            _recurring.AddOrUpdate(jobId, job, cron, new RecurringJobOptions()
            {
                TimeZone = TimeZoneInfo.Utc
            });
        }

        public string Enqueue<T>(Expression<Func<T, Task>> job)
        {
           return  _client.Enqueue(job);
        }

        public string Schedule<T>(Expression<Func<T, Task>> job, TimeSpan delay)
        {
            return _client.Schedule(job,delay);
        }
    }
}
