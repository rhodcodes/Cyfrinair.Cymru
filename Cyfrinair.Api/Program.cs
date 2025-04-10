
using System.Security.Cryptography;
using System.Text;

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

app.MapGet("/password/{length}", (int length = 5, [FromQuery(Name = "sc")] bool includeSpecialChars = false) =>
{
    var characterSet = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
    var specialChars = "!@#$%^&*()_+[]{}|;:,.<>?";
    var result = new StringBuilder(length);
    var bytes = new byte[length];
    
    if (includeSpecialChars)
    {
        characterSet += specialChars;
    }
    
    using (var rng = RandomNumberGenerator.Create())
    {
        rng.GetBytes(bytes);
    }

    for (int i = 0; i < length; i++)
    {
        int index = bytes[i] % characterSet.Length;
        result.Append(characterSet[index]);
    }
    return result.ToString();
}).WithName("GetPassword");

app.MapGet("/guid", () => Guid.NewGuid().ToString("D")).WithName("GetGuid");

app.Run();