using Microsoft.Extensions.Logging;

namespace Todo.Application.Jobs
{
    public class CleanupPendingTaskJob
    {
        private readonly ILogger<CleanupPendingTaskJob> _logger;

        public CleanupPendingTaskJob(ILogger<CleanupPendingTaskJob> logger)
        {
            this._logger = logger;
        }



        public async Task CleanUpPendingTodos()
        {
            _logger.LogInformation($"Executing cleanup pending Todo Jobs for User Id {DateTime.UtcNow}");
            await Task.Delay(5000);
        }
    }
}
