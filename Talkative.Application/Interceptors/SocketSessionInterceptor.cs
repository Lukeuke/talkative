using HotChocolate.AspNetCore;
using HotChocolate.AspNetCore.Subscriptions;
using HotChocolate.AspNetCore.Subscriptions.Protocols;
using Talkative.Application.Helpers;

namespace Talkative.Application.Interceptors;

public class SocketSessionInterceptor : DefaultSocketSessionInterceptor
{
    public override async ValueTask<ConnectionStatus> OnConnectAsync(ISocketSession session, IOperationMessagePayload connectionInitMessage,
        CancellationToken cancellationToken = new CancellationToken())
    {
        var authorization = session.Connection.HttpContext.Request.Headers["Authorization"];

        foreach (var (key, value) in session.Connection.HttpContext.Request.Headers)
        {
            Console.WriteLine(key);
        }

        if (authorization.Count != 0)
        {
            var token = authorization[0]!.Split(" ")[1];

            if (token.Validate())
            {
                return ConnectionStatus.Accept();
            }
        }

        return ConnectionStatus.Reject("Authorization Header is missing or token is invalid or expired");
    }
}