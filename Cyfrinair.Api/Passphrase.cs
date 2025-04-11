using System.Collections.Immutable;
using System.Globalization;
using System.Security.Cryptography;

namespace Cyfrinair.Api;

public static class Passphrase
{
    private static ImmutableArray<string> s_wordList = ImmutableArray.Create("cariad", "dŵr", "hedyn", "tân", "iach", "glan",
        "bywyd", "llawenydd", "canol", "modd", "hwyl", "pethau", "ynys", "deffro", "mwyn", "dawn", "plentyn", "athro",
        "canu", "cyfrinach", "egwyl", "gwaith", "hoff", "llachar");
    
    private static readonly TextInfo textInfo = new CultureInfo("cy-GB").TextInfo;

    public static string Generate(PassphraseOptions options)
    {
        var candidates = new string[options.Words];

        for (var i = 0; i < options.Words; i++)
        {
            var r = RandomNumberGenerator.GetInt32(s_wordList.Length);
            candidates[i] = s_wordList[r];
        }

        if (options.Casing ==
            // Word Casing
            CasingOption.Upper)
        {
            for (int i = 0; i < candidates.Length; i++)
            {
                candidates[i] = textInfo.ToTitleCase(candidates[i]);
            }
        }
        
        if (options.Casing == CasingOption.Lower)
        {
            for (int i = 0; i < candidates.Length; i++)
            {
                candidates[i] = textInfo.ToLower(candidates[i]);
            }
        }
        
        if (options.Casing == CasingOption.Random)
        {
            for (int i = 0; i < candidates.Length; i++)
            {
                int r = RandomNumberGenerator.GetInt32(2);
                if (r == 0)
                {
                    candidates[i] = textInfo.ToTitleCase(candidates[i]);
                }
                else
                {
                    candidates[i] = textInfo.ToLower(candidates[i]);
                }
            }
        }
        
        // Add digits

        if (options.DigitPlacement == DigitPlacementOption.Once)
        {
            int index = RandomNumberGenerator.GetInt32(candidates.Length);
            candidates[index] = AppendRandomNumber(candidates[index]);
        }

        if (options.DigitPlacement == DigitPlacementOption.EveryWord)
        {
            for (int i = 0; i < candidates.Length; i++)
            {
                candidates[i] = AppendRandomNumber(candidates[i]);
            }
        }
        
        // Add separator
        var result = string.Join(options.Separator, candidates);
        
        return result;
    }

    private static string AppendRandomNumber(string candidate)
    {
        return $"{candidate}{RandomNumberGenerator.GetInt32(10)}";
    }
}

public record PassphraseOptions
{
    public int Words { get; init; } = 4;
    public char Separator { get; init; } = '-';
    public DigitPlacementOption DigitPlacement { get; init; } = DigitPlacementOption.Once;
    public CasingOption Casing { get; init; } = CasingOption.Upper;
}

public enum CasingOption
{
    Upper,
    Lower,
    Random,
}

public enum DigitPlacementOption
{
    Once,
    None,
    EveryWord,
}