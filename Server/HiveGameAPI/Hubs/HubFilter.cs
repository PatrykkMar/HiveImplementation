using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace HiveGame.Hubs
{


    public class ExceptionHandlingHubFilter : IHubFilter
    {
        private readonly ILogger<ExceptionHandlingHubFilter> _logger;

        public ExceptionHandlingHubFilter(ILogger<ExceptionHandlingHubFilter> logger)
        {
            _logger = logger;
        }

        public async ValueTask<object?> InvokeMethodAsync(
            HubInvocationContext invocationContext,
            Func<HubInvocationContext, ValueTask<object>> next)
        {
            try
            {
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
