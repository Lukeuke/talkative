using Talkative.Domain.Models;

namespace Talkative.Worker;

public interface IBackgroundTaskQueue
{
    void EnqueueTask(UserStatus task);
    Task<UserStatus> DequeueTaskAsync(CancellationToken cancellationToken);
}