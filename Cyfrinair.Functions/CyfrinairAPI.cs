using System.Net;

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

    [OpenApiOperation("PasswordGen", "Password")]
    [OpenApiParameter("count", In = ParameterLocation.Path, Required = false, Type = typeof(int),
        Description = "The number of passwords to generate. Defaults to 1.")]
    [OpenApiParameter("numbers", In = ParameterLocation.Query, Required = false, Type = typeof(bool),
        Description = "Include digits in the password. Defaults to true.")]
    [OpenApiParameter("symbols", In = ParameterLocation.Query, Required = false, Type = typeof(bool),
        Description = "Include symbols in the password. Defaults to true.")]
    [OpenApiParameter("ambiguous", In = ParameterLocation.Query, Required = false, Type = typeof(bool),
        Description = "Include ambiguous characters (i,l,1,o,0 etc) in the password. Defaults to true.")]
    [OpenApiParameter("length", In = ParameterLocation.Query, Required = false, Type = typeof(int),
        Description = "The length of the generated passwords. Defaults to 12.")]
    [OpenApiResponseWithBody(HttpStatusCode.OK, "application/json", typeof(List<string>),
        Description = "A list of generated passwords.")]
    [Function("Password")]
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
        int length = int.TryParse(req.Query["length"], out int parsedLength)
            ? parsedLength
            : 12;
        PasswordOptions options = new()
        {
            IncludeDigits = includeDigits,
            IncludeSymbols = includeSymbols,
            IncludeAmbiguousChars = includeAmbiguousChars,
            Length = length
        };

        Password password = new(options);
        List<string> result = password.Generate((ushort)count);
        return new OkObjectResult(result);
    }

    [OpenApiOperation("PassphraseGen", "Passphrase")]
    [OpenApiParameter("count", In = ParameterLocation.Path, Required = false, Type = typeof(int),
        Description = "The number of passphrases to generate. Defaults to 1.")]
    [OpenApiParameter("words", In = ParameterLocation.Query, Required = false, Type = typeof(int),
        Description = "The number of words in each passphrase. Defaults to 4.")]
    [OpenApiParameter("separator", In = ParameterLocation.Query, Required = false, Type = typeof(char),
        Description = "The character used to separate words in the passphrase. Defaults to '-'.")]
    [OpenApiParameter("casing", In = ParameterLocation.Query, Required = false, Type = typeof(string),
        Description = "The casing of the passphrase. Options are 'upper', 'lower', or 'random'. Defaults to 'upper'.")]
    [OpenApiParameter("digit", In = ParameterLocation.Query, Required = false, Type = typeof(string),
        Description =
            "The placement of digits in the passphrase. Options are 'once', 'none', or 'every'. Defaults to 'once'.")]
    [OpenApiResponseWithBody(HttpStatusCode.OK, "application/json", typeof(List<string>),
        Description = "A list of generated passphrases.")]
    [Function("Passphrase")]
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

    [Function("RedirectToApiDocs")]
    public IActionResult RedirectToApiDocs(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "{ignored:maxlength(0)?}")]
        HttpRequest req,
        string ingored = "")
    {
        return new RedirectResult("/swagger/ui", false);
    }
}