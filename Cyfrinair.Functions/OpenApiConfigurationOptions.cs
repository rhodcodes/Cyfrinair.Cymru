using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Abstractions;
using Microsoft.Azure.WebJobs.Extensions.OpenApi.Core.Enums;
using Microsoft.OpenApi.Models;

namespace Cyfrinair.Functions;
public class OpenApiConfigurationOptions : IOpenApiConfigurationOptions
{
    public OpenApiInfo Info { get; set; } =
        new OpenApiInfo
        {
            Title = "Cyfrinair.Cymru",
            Version = "0.0.1-beta",
            Description = "API for generating passwords and passphrases using the Welsh language. Part of the Cyfrinair.cymru project.",
            Contact = new OpenApiContact()
            {
                Name = "Rhodri Lloyd-Evans",
                Url = new Uri("https://github.com/rhodcodes/Cyfrinair.Cymru/issues/"),
            },
            License = new OpenApiLicense()
            {
                Name = "MIT",
                Url = new Uri("http://opensource.org/licenses/MIT"),
            }
        };

    public List<OpenApiServer> Servers { get; set; } = new();

    public OpenApiVersionType OpenApiVersion { get; set; } = OpenApiVersionType.V2;

    public bool IncludeRequestingHostName { get; set; } = false;
    public bool ForceHttp { get; set; } = true;
    public bool ForceHttps { get; set; } = false;
    public List<IDocumentFilter> DocumentFilters { get; set; } = new();
}