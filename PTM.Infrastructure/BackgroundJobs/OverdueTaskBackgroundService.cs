using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PTM.Infrastructure.Repositories.Interfaces;
using PTM.Domain.Entities.Enums;

namespace PTM.Infrastructure.BackgroundJobs;

public class OverdueTaskBackgroundService(IServiceProvider serviceProvider, ILogger<OverdueTaskBackgroundService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Overdue Task Background Service is running....");

        while (!stoppingToken.IsCancellationRequested)
        {
            logger.LogInformation("Overdue Task Background Service is running.....");

            using (var scope = serviceProvider.CreateScope())
            {
                var commiter = scope.ServiceProvider.GetRequiredService<IEntityCommiter>();
                try
                {
                    var overdueTasksResult = await commiter.Tasks.GetAllAsync(t => t.Status == AppTaskStatus.Pending && t.DueDate < DateTime.UtcNow);

                    if (overdueTasksResult.IsSuccess && overdueTasksResult.Data != null)
                    {
                        foreach (var task in overdueTasksResult.Data)
                        {
                            task.Status = AppTaskStatus.Overdue;
                            await commiter.Tasks.UpdateAsync(task);
                            logger.LogInformation($"Task {task.Id} marked as Overdue........");
                        }
                        await commiter.CommitAsync(stoppingToken);
                    }
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error occurred while marking tasks as overdue..........");
                }
            }
            await Task.Delay(TimeSpan.FromHours(1), stoppingToken);
        }

        logger.LogInformation("Overdue BackgroundService has been stopped......");
    }
} 