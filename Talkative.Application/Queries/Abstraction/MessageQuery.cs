using Microsoft.AspNetCore.Http;
using Talkative.Domain.Entities;
using Talkative.Infrastructure.Context;

namespace Talkative.Application.Queries.Abstraction;

public interface IMessageQuery
{
    public IEnumerable<Message> GetAllMessages(Guid groupId, ApplicationContext context, IHttpContextAccessor httpContextAccessor);
}