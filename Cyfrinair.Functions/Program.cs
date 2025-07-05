using Microsoft.Azure.Functions.Worker.Extensions.OpenApi.Extensions;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureServices((context, services) =>
    {
        // Add any additional services here if needed
    })
    .ConfigureFunctionsWebApplication(app =>
    {
        // Configure the application here if needed
    })
    .ConfigureOpenApi()
    .Build();
host.Run();