namespace Cyfrinair.Core;

public static class Converters
{
    public static CasingOption ToCasingOption(this string casing)
    {
        return casing.ToLower() switch
        {
            "upper" => CasingOption.Upper,
            "lower" => CasingOption.Lower,
            "random" => CasingOption.Random,
            _ => throw new ArgumentException($"Invalid casing option: {casing}")
        };
    }
    
    public static DigitPlacementOption ToDigitPlacementOption(this string digitPlacement)
    {
        return digitPlacement.ToLower() switch
        {
            "once" => DigitPlacementOption.Once,
            "none" => DigitPlacementOption.None,
            "every" => DigitPlacementOption.EveryWord,
            _ => throw new ArgumentException($"Invalid digit placement option: {digitPlacement}")
        };
    }
}