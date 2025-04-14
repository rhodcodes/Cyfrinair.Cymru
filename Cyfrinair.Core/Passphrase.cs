using System.Collections.Immutable;
using System.Globalization;
using System.Security.Cryptography;

namespace Cyfrinair.Core;

public class Passphrase(PassphraseOptions options)
{
    private readonly PassphraseOptions _options = options ?? throw new ArgumentNullException(nameof(options));

    private readonly ImmutableArray<string> _wordList = ImmutableArray.Create("cariad", "dŵr", "hedyn", "tân", "iach", "glan",
        "bywyd", "llawenydd", "canol", "modd", "hwyl", "pethau", "ynys", "deffro", "mwyn", "dawn", "plentyn", "athro",
        "canu", "cyfrinach", "egwyl", "gwaith", "hoff", "llachar");
    
    private readonly TextInfo _textInfo = new CultureInfo("cy-GB").TextInfo;

    public List<string> Generate(ushort quantity)
    {
        var results = new List<string>(quantity);

        for (int i = 0; i < quantity; i++)
        {
            results.Add(CreatePassphrase());
        }
        return results;
    }

    private string CreatePassphrase()
    {
        var candidates = new string[_options.Words];

        for (var i = 0; i < _options.Words; i++)
        {
            var r = RandomNumberGenerator.GetInt32(_wordList.Length);
            candidates[i] = _wordList[r];
        }

        if (_options.Casing ==
            // Word Casing
            CasingOption.Upper)
        {
            for (int i = 0; i < candidates.Length; i++)
            {
                candidates[i] = _textInfo.ToTitleCase(candidates[i]);
            }
        }
        
        if (_options.Casing == CasingOption.Lower)
        {
            for (int i = 0; i < candidates.Length; i++)
            {
                candidates[i] = _textInfo.ToLower(candidates[i]);
            }
        }
        
        if (_options.Casing == CasingOption.Random)
        {
            for (int i = 0; i < candidates.Length; i++)
            {
                int r = RandomNumberGenerator.GetInt32(2);
                if (r == 0)
                {
                    candidates[i] = _textInfo.ToTitleCase(candidates[i]);
                }
                else
                {
                    candidates[i] = _textInfo.ToLower(candidates[i]);
                }
            }
        }
        
        // Add digits

        if (_options.DigitPlacement == DigitPlacementOption.Once)
        {
            int index = RandomNumberGenerator.GetInt32(candidates.Length);
            candidates[index] = AppendRandomNumber(candidates[index]);
        }

        if (_options.DigitPlacement == DigitPlacementOption.EveryWord)
        {
            for (int i = 0; i < candidates.Length; i++)
            {
                candidates[i] = AppendRandomNumber(candidates[i]);
            }
        }
        
        // Add separator
        var passphrase = string.Join(_options.Separator, candidates);
        
        return passphrase;
    }

    private string AppendRandomNumber(string candidate)
    {
        return $"{candidate}{RandomNumberGenerator.GetInt32(10)}";
    }
}