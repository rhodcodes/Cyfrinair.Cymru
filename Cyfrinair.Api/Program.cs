
using System.Security.Cryptography;
using System.Text;

using Cyfrinair.Api;

using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

Passwords.Map(app);



app.MapGet("/passphrase", () =>
    {
        var options = new PassphraseOptions();
        var result = Passphrase.Generate(options);
        app.Logger.LogInformation(result);
        return result;
    })
    .WithName("GetPassphrase");

app.MapGet("/guid", () => Guid.NewGuid().ToString("D")).WithName("GetGuid");

app.Run();