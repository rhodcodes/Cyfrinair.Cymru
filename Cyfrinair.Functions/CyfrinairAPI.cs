using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Cyfrinair.Functions
{
    public class CyfrinairAPI
    {
        private readonly ILogger<CyfrinairAPI> _logger;

        public CyfrinairAPI(ILogger<CyfrinairAPI> logger)
        {
            _logger = logger;
        }

        [Function("CyfrinairAPI")]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Function, "get", "post")] HttpRequest req)
        {
            _logger.LogInformation("C# HTTP trigger function processed a request.");
            return new OkObjectResult("Welcome to Azure Functions!");
        }
    }
}
