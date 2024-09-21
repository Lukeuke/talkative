using Talkative.Domain.Entities;

namespace Talkative.Application.Queries.Abstraction;

public interface IMessageQuery
{
    public IEnumerable<Message> GetAllMessages(Guid groupId);
}