namespace HiveGame.MIddlewares
{
    public class ResponseSizeLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ResponseSizeLoggingMiddleware> _logger;

        public ResponseSizeLoggingMiddleware(RequestDelegate next, ILogger<ResponseSizeLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var originalBodyStream = context.Response.Body;

            try
            {
                using (var memoryStream = new MemoryStream())
                {
                    context.Response.Body = memoryStream;

                    await _next(context);

                    memoryStream.Seek(0, SeekOrigin.Begin);
                    var responseBody = await new StreamReader(memoryStream).ReadToEndAsync();
                    memoryStream.Seek(0, SeekOrigin.Begin);

                    var responseSize = memoryStream.Length;
                    _logger.LogInformation($"Response size: {responseSize/1024} kilobytes");

                    await memoryStream.CopyToAsync(originalBodyStream);
                }
            }
            finally
            {
                context.Response.Body = originalBodyStream;
            }
        }
    }
}
