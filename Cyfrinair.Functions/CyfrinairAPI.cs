using Cyfrinair.Core;

using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Cyfrinair.Functions
{
    public class CyfrinairApi(ILogger<CyfrinairApi> logger)
    {
        private readonly ILogger<CyfrinairApi> _logger = logger;

        [Function("Cyfrinair")]
        public static IActionResult Password(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "password/{count:int}")] HttpRequest req,
            int count)
        {
            // TODO: Add validation for count
            // TODO: Add query parameters for length, includeDigits, includeSymbols, includeAmbiguousChars and validation.
            var options = new PasswordOptions()
            {
                IncludeDigits = true,
                IncludeSymbols = false,
                IncludeAmbiguousChars = false
            };
            
            var password = new Password(options);
            var result = password.Generate((ushort)count);
            return new OkObjectResult(result);
        }

        [Function("BrawddegCudd")]
        public static IActionResult Passphrase(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "passphrase/{count:int}")] HttpRequest req,
            int count)
        {
            // TODO: Add validation for count
            // TODO: Add query parameters for words, separator, digit and casing and validation.
            var options = new PassphraseOptions();
            var passphrase = new Passphrase(options);
            var result = passphrase.Generate((ushort)count);
            return new OkObjectResult(result);
        }
    }
}
