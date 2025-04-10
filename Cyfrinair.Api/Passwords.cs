using System.Security.Cryptography;
using System.Text;

using Microsoft.AspNetCore.Mvc;

namespace Cyfrinair.Api;

public static class Passwords
{
    public static void Map(WebApplication app)
    {
        app.MapGet("/password", (
            [FromQuery(Name="length")] int length = 25,
            [FromQuery(Name="digits")] bool includeDigits = true,
            [FromQuery(Name = "symbols")] bool includeSymbols = false,
            [FromQuery(Name = "ambiguous")] bool includeAmbiguousChars = false) =>
        {
            app.Logger.LogInformation(
                string.Format(
                    "PWD generated - length: {0}, includeDigits: {1}, includeSymbols: {2}, includeAmbiguousChars: {3}",
                    length, includeDigits, includeSymbols, includeAmbiguousChars));
            return GeneratePassword(length, includeSymbols, includeDigits, includeAmbiguousChars);
        }).WithName("GetPassword");
        
        app.MapGet("/password/{n}", (
            int n,
            [FromQuery(Name = "length")] int length = 25,
            [FromQuery(Name="digits")] bool includeDigits = true,
            [FromQuery(Name="symbols")] bool includeSymbols = false,
            [FromQuery(Name = "ambiguous")] bool includeAmbiguousChars = false) =>
        {
            var results = new StringBuilder();

            for (var i = 0; i < n; i++)
            {
                results.AppendLine(GeneratePassword(length, includeSymbols, includeDigits, includeAmbiguousChars));
            }
            
            return results.ToString();
        }).WithName("GetPasswords");
    }

    private static string GeneratePassword(int length, bool includeSymbols, bool includeDigits, bool includeAmbiguousChars)
    {
        var baseCharacterSet = "abcdefghjkmnpqrtuvwxyzACDEFGHIJKMNPQRTUVWXYZ";
        const string digitSet = "2345679";
        const string symbolSet = "!@#$%^&*()+[]{}<>?";
        const string ambiguousChars = "ilLoOSsB";
        const string ambiguousSymbols = "-_.,|";
        const string ambiguousDigits = "0158";
        var result = new StringBuilder(length);
        var bytes = new byte[length];
    
        if (includeSymbols)
        {
            baseCharacterSet += symbolSet;
        }

        if (includeDigits)
        {
            baseCharacterSet += digitSet;
        }

        if (includeAmbiguousChars)
        {
            baseCharacterSet += ambiguousChars;
        }
        
        if (includeSymbols && includeAmbiguousChars)
        {
            baseCharacterSet += ambiguousSymbols;
        }
        
        if (includeDigits && includeAmbiguousChars)
        {
            baseCharacterSet += ambiguousDigits;
        }
    
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(bytes);
        }

        for (int i = 0; i < length; i++)
        {
            int index = bytes[i] % baseCharacterSet.Length;
            result.Append(baseCharacterSet[index]);
        }
            
        return result.ToString();
    }
}