using System.Security.Cryptography;
using System.Text;

namespace Cyfrinair.Core;

public class Password(PasswordOptions options)
{
    private readonly PasswordOptions _options = options ?? throw new ArgumentNullException(nameof(options));
    private const string BaseCharacterSet = "abcdefghjkmnpqrtuvwxyzACDEFGHIJKMNPQRTUVWXYZ";
    private const string DigitSet = "2345679";
    private const string SymbolSet = "!@#$%^&*()+[]{}<>?";
    private const string AmbiguousChars = "ilLoOSsB";
    private const string AmbiguousSymbols = "-_.,|";
    private const string AmbiguousDigits = "0158";

    public List<string> Generate(ushort quantity)
    {
        var results = new List<string>(quantity);
        for (var i = 1; i <= quantity; i++)
        {
            results.Add(CreatePassword());
        }
        
        return results;
    }

    private string CreatePassword()
    {
        var availableCharacters = BaseCharacterSet;
        var result = new StringBuilder(_options.Length);
        var bytes = new byte[_options.Length];
    
        if (_options.IncludeSymbols)
        {
            availableCharacters += SymbolSet;
        }

        if (_options.IncludeDigits)
        {
            availableCharacters += DigitSet;
        }

        if (_options.IncludeAmbiguousChars)
        {
            availableCharacters += AmbiguousChars;
        }
        
        if (_options is { IncludeSymbols: true, IncludeAmbiguousChars: true })
        {
            availableCharacters += AmbiguousSymbols;
        }
        
        if (_options is { IncludeDigits: true, IncludeAmbiguousChars: true })
        {
            availableCharacters += AmbiguousDigits;
        }
    
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(bytes);
        }

        for (int i = 0; i < _options.Length; i++)
        {
            int index = bytes[i] % availableCharacters.Length;
            result.Append(availableCharacters[index]);
        }
            
        return result.ToString();
    }
}