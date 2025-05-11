using Cyfrinair.Core;

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

app.MapGet("/password/{qty:int?}", (
    int qty = 1,
    [FromQuery(Name = "length")] int length = 25,
    [FromQuery(Name="digits")] bool includeDigits = true,
    [FromQuery(Name="symbols")] bool includeSymbols = false,
    [FromQuery(Name = "ambiguous")] bool includeAmbiguousChars = false) =>
{
    var options = new PasswordOptions() {
        Length = length,
        IncludeDigits = includeDigits,
        IncludeSymbols = includeSymbols,
        IncludeAmbiguousChars = includeAmbiguousChars
    };
    var password = new Password(options);
    return Results.Ok(password.Generate((ushort)qty));
}).WithName("GetPassword");

app.MapGet("/passphrase/{qty:int?}", (
    int qty = 1,
    [FromQuery(Name="words")] int words = 4,
    [FromQuery(Name="separator")] string separator = "-",
    [FromQuery(Name = "digit")] string digit = "once",
    [FromQuery(Name = "casing")] string casing = "upper") =>
    {
        var options = new PassphraseOptions
        {
            Words = words,
            Separator = separator[0],
            DigitPlacement = digit.ToDigitPlacementOption(),
            Casing = casing.ToCasingOption(),
        };
        var passphrase = new Passphrase(options);
        var result = passphrase.Generate((ushort)qty);
        return Results.Ok(result);
    })
    .WithName("GetPassphrase");

app.MapGet("/guid", () => Guid.NewGuid().ToString("D")).WithName("GetGuid");

app.Run();