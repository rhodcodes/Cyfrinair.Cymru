using Cyfrinair.Core;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Attributes;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

namespace Cyfrinair.Functions;

public class CyfrinairApi(ILogger<CyfrinairApi> logger)
{
    private readonly ILogger<CyfrinairApi> _logger = logger;

    [Function("Cyfrinair")]
    public static IActionResult Password(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "password/{count:int?}")]
        HttpRequest req,
        int count = 1)
    {
        switch (count)
        {
            case < 1:
                return new BadRequestObjectResult(new
                {
                    error = "InvalidCount", message = "You must generate at least one password."
                });
            case > ushort.MaxValue:
                return new BadRequestObjectResult(new
                {
                    error = "InvalidCount", message = "You can only generate up to 65535 passwords at a time."
                });
        }

        bool includeDigits = !bool.TryParse(req.Query["numbers"], out bool parsedValue) || parsedValue;
        bool includeSymbols = !bool.TryParse(req.Query["symbols"], out parsedValue) || parsedValue;
        bool includeAmbiguousChars = !bool.TryParse(req.Query["ambiguous"], out parsedValue) || parsedValue;
        PasswordOptions options = new()
        {
            IncludeDigits = includeDigits,
            IncludeSymbols = includeSymbols,
            IncludeAmbiguousChars = includeAmbiguousChars
        };

        Password password = new(options);
        List<string> result = password.Generate((ushort)count);
        return new OkObjectResult(result);
    }
    
    [OpenApiOperation(operationId:"BrawddegCudd", tags: ["Passphrase"])]
    [OpenApiParameter(name: "count", In = ParameterLocation.Path, Required = false, Type = typeof(int), Description = "The number of passphrases to generate. Defaults to 1.")]
    [OpenApiParameter(name: "words", In = ParameterLocation.Query, Required = false, Type = typeof(int), Description = "The number of words in each passphrase. Defaults to 4.")]
    [OpenApiParameter(name: "separator", In = ParameterLocation.Query, Required = false, Type = typeof(string), Description = "The character used to separate words in the passphrase. Defaults to '-'.")]
    [OpenApiParameter(name: "casing", In = ParameterLocation.Query, Required = false, Type = typeof(string), Description = "The casing of the passphrase. Options are 'upper', 'lower', or 'random'. Defaults to 'upper'.")]
    [OpenApiParameter(name: "digit", In = ParameterLocation.Query, Required = false, Type = typeof(string), Description = "The placement of digits in the passphrase. Options are 'once', 'none', or 'everyword'. Defaults to 'once'.")]
    [Function("BrawddegCudd")]
    public static IActionResult Passphrase(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "passphrase/{count:int?}")]
        HttpRequest req,
        int count = 1)
    {
        switch (count)
        {
            case < 1:
                return new BadRequestObjectResult(new
                {
                    error = "InvalidCount", message = "You must generate at least one passphrase."
                });
            case > ushort.MaxValue:
                return new BadRequestObjectResult(new
                {
                    error = "InvalidCount", message = "You can only generate up to 65535 passphrases at a time."
                });
        }

        int words = int.TryParse(req.Query["words"], out int parsedValue)
            ? parsedValue
            : 4;
        char separator = !string.IsNullOrEmpty(req.Query["separator"])
            ? req.Query["separator"].ToString()[0]
            : '-';
        CasingOption casing = (!string.IsNullOrEmpty(req.Query["casing"])
            ? req.Query["casing"].ToString()
            : "upper").ToCasingOption();
        DigitPlacementOption digitPlacement = (!string.IsNullOrEmpty(req.Query["digit"])
            ? req.Query["digit"].ToString()
            : "once").ToDigitPlacementOption();

        PassphraseOptions options = new()
        {
            Words = words, Separator = separator, Casing = casing, DigitPlacement = digitPlacement
        };
        Passphrase passphrase = new(options);
        List<string> result = passphrase.Generate((ushort)count);
        return new OkObjectResult(result);
    }
}