using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Abstractions;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.OpenApi.Models;

namespace Cyfrinair.Functions;

public class OpenApiConfigurationOptions : IOpenApiConfigurationOptions
{
    public OpenApiInfo Info { get; set; } =
        new()
        {
            Title = "Cyfrinair.Cymru",
            Version = "0.0.1-beta",
            Description =
                "API ar gyfer creu cyfrineiriau neu brawddegau-cudd yn yr iaith Gymraeg. " +
                "Rhan o prosiect <a href=\"https://cyfrinair.cymru\">Cyfrinair.cymru</a>.<br>" +
                "API for generating passwords and passphrases using the Welsh language. " +
                "Part of the <a href=\"https://cyfrinair.cymru\">Cyfrinair.cymru</a> project.",
            Contact = new OpenApiContact
            {
                Name = "Rhodri Lloyd-Evans", Url = new Uri("https://github.com/rhodcodes/Cyfrinair.Cymru/issues/")
            },
            License = new OpenApiLicense { Name = "MIT", Url = new Uri("https://opensource.org/licenses/MIT") }
        };

    public List<OpenApiServer> Servers { get; set; } = new();

    public OpenApiVersionType OpenApiVersion { get; set; } = OpenApiVersionType.V2;

    public bool IncludeRequestingHostName { get; set; } = false;
    public bool ForceHttp { get; set; } = true;
    public bool ForceHttps { get; set; } = false;
    public List<IDocumentFilter> DocumentFilters { get; set; } = new();
}