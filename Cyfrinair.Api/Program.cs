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

app.MapGet("/password", (
    [FromQuery(Name="length")] int length = 25,
    [FromQuery(Name="digits")] bool includeDigits = true,
    [FromQuery(Name = "symbols")] bool includeSymbols = false,
    [FromQuery(Name = "ambiguous")] bool includeAmbiguousChars = false) =>
{
    var passwordOptions = new PasswordOptions(length, includeDigits, includeSymbols, includeAmbiguousChars);
    var password = new Password(passwordOptions);
    return Results.Ok(password.Generate(1));
}).WithName("GetPassword");
        
app.MapGet("/password/{n}", (
    int n,
    [FromQuery(Name = "length")] int length = 25,
    [FromQuery(Name="digits")] bool includeDigits = true,
    [FromQuery(Name="symbols")] bool includeSymbols = false,
    [FromQuery(Name = "ambiguous")] bool includeAmbiguousChars = false) =>
{
    var passwordOptions = new PasswordOptions(length, includeDigits, includeSymbols, includeAmbiguousChars);
    var password = new Password(passwordOptions);
    return Results.Ok(password.Generate((ushort)n));
}).WithName("GetPasswords");



app.MapGet("/passphrase/{n}", (
    int n = 1,
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
        var result = passphrase.Generate((ushort)n);
        return Results.Ok(result);
    })
    .WithName("GetPassphrase");

app.MapGet("/guid", () => Guid.NewGuid().ToString("D")).WithName("GetGuid");

app.Run();