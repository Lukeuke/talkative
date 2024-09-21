using HotChocolate.Subscriptions;
using Microsoft.AspNetCore.Http;
using Talkative.Domain.Entities;
using Talkative.Infrastructure.Context;

namespace Talkative.Application.Mutations.Abstraction;

public interface IMessageMutation
{
    Task<Message> SendMessage(Guid roomId, string content, ApplicationContext context, IHttpContextAccessor httpContextAccessor, ITopicEventSender sender);
}