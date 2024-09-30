using HotChocolate.Subscriptions;
using Talkative.Domain.Models;
using Talkative.Infrastructure.Helpers;

namespace Talkative.Worker;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IBackgroundTaskQueue _taskQueue;
    private readonly UserStatusHelper _userStatusHelper;
    private readonly ITopicEventSender _eventSender;

    public Worker(
        ILogger<Worker> logger, 
        IBackgroundTaskQueue taskQueue, 
        UserStatusHelper userStatusHelper,
        ITopicEventSender eventSender
        )
    {
        _logger = logger;
        _taskQueue = taskQueue;
        _userStatusHelper = userStatusHelper;
        _eventSender = eventSender;
    }

    private Dictionary<Guid, DateTime> ConnectedUsers { get; set; } = new();
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            //_logger.LogInformation("Attempting to dequeue a task at {time}", DateTimeOffset.UtcNow);
            var userStatus = await _taskQueue.DequeueTaskAsync(stoppingToken);
            
            if (userStatus == null)
            {
                //_logger.LogWarning("No tasks in the queue. Waiting for new tasks...");
            }
            else
            {
                _logger.LogInformation("Dequeued userStatus: {userId}", userStatus.UserId);

                ConnectedUsers[userStatus.UserId] = DateTime.UtcNow;
            }

            // Users who are not sent a ping for more than 30 seconds
            var usersWhoDisconnected = ConnectedUsers.Where(x => x.Value < DateTimeOffset.UtcNow.AddSeconds(-30));

            foreach (var (userId, _) in usersWhoDisconnected)
            {
                ConnectedUsers.Remove(userId);
                await _userStatusHelper.UpdateUserStatus(userId, false);
                await _eventSender.SendAsync("UserStatusChanged", new UserStatus
                {
                    UserId = userId,
                    IsOnline = false
                }, stoppingToken);
                
                _logger.LogInformation("User status set to false: {userId}", userId);
            }

            if (userStatus is not null)
            {
                _logger.LogInformation(userStatus.ToString());
            }
            // _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);

            await Task.Delay(1000, stoppingToken);
        }
    }
}