using Cyfrinair.Core;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

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