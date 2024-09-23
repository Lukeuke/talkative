using System.Collections.Concurrent;
using Talkative.Domain.Models;

namespace Talkative.Worker;

public class BackgroundTaskQueue : IBackgroundTaskQueue
{
    private readonly ILogger<BackgroundTaskQueue> _logger;
    private readonly ConcurrentQueue<UserStatus> _tasks = new();
    private readonly SemaphoreSlim _signal = new(0);

    public BackgroundTaskQueue(ILogger<BackgroundTaskQueue> logger)
    {
        _logger = logger;
    }
    
    public void EnqueueTask(UserStatus task)
    {
        _logger.LogInformation("Enqueuing task for user: {UserId}", task.UserId);
        _tasks.Enqueue(task);
        _signal.Release();
    }

    public async Task<UserStatus> DequeueTaskAsync(CancellationToken cancellationToken)
    {
        if (await _signal.WaitAsync(TimeSpan.FromSeconds(1), cancellationToken)) // 5-second timeout
        {
            _tasks.TryDequeue(out var taskInfo);
            return taskInfo ?? new UserStatus
            {
                UserId = default,
                IsOnline = false
            };
        }

        // Log the timeout scenario
        return new UserStatus
        {
            UserId = default,
            IsOnline = false // Return a default state when timing out
        };
    }
}