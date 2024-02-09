using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;

namespace PsGenApi
{
    public class HealthCheckFunction : FunctionBase
    {
        private readonly ILogger _logger;

        public HealthCheckFunction(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<UsersFunction>();
        }

        [Function("health-check")]
        public HttpResponseData Run([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req)
        {
            _logger.LogInformation("Executing health check function.");

            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "text/plain; charset=utf-8");

            response.WriteString("Function is running!");

            return response;
        }
    }
}
