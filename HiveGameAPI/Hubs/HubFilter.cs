using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace HiveGame.Hubs
{


    public class HubFilter : IHubFilter
    {
        private readonly ILogger<HubFilter> _logger;

        public HubFilter(ILogger<HubFilter> logger)
        {
            _logger = logger;
        }

        public async ValueTask<object?> InvokeMethodAsync(
            HubInvocationContext invocationContext,
            Func<HubInvocationContext, ValueTask<object>> next)
        {
            try
            {
                _logger.LogInformation("SignalR Request: Hub: {HubName}, Method: {MethodName}, Arguments: {Arguments}",
                invocationContext.HubMethodName,
                invocationContext.Context.UserIdentifier,
                invocationContext.HubMethodArguments);
                return await next(invocationContext);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception in hub method: {MethodName}", invocationContext.HubMethodName);

                await invocationContext.Hub.Clients.Caller.SendAsync("ReceiveError",
                    $"An error occurred: {ex.Message}");

                return null;
            }
        }
    }
}
