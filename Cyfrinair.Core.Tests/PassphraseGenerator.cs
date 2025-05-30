using TUnit.Assertions.AssertConditions.Throws;

namespace Cyfrinair.Core.Tests;

public class PassphraseGenerator
{
    [Test]
    public async Task ThrowsArgumentNullException_GivenNullOptions()
    {
        // Arrange
        PassphraseOptions options = null!;

        // Act & Assert
        await Assert.That(() =>
        {
            new Passphrase(options);
        }).Throws<ArgumentNullException>();
    }
    
    [Test]
    public async Task ReturnsSinglePassphrase_GivenOneCount()
    {
        // Arrange
        var options = new PassphraseOptions();
        var generator = new Passphrase(options);
        
        // Act
        var result = generator.Generate(1);
        
        // Assert
        await Assert.That(result).HasSingleItem();
    }

    [Test]
    [Arguments(3)]
    [Arguments(4)]
    [Arguments(5)]
    [Arguments(6)]
    [Arguments(7)]
    
    public async Task ReturnsMultipleDistinctPassphrases_GivenMultipleCount(ushort n)
    {
        // Arrange
        var options = new PassphraseOptions();
        var generator = new Passphrase(options);
        
        // Act
        var result = generator.Generate(n);
        
        // Assert
        await Assert.That(result).HasCount(n).And.HasDistinctItems();
    }
    
    [Test]
    [Arguments(2)]
    [Arguments(4)]
    [Arguments(6)]
    [Arguments(8)]
    [Arguments(10)]
    [Arguments(12)]
    [Arguments(14)]
    public async Task ReturnsNWords_GivenNWordsOption(int n)
    {
        // Arrange
        var options = new PassphraseOptions { Words = n };
        var generator = new Passphrase(options);
        
        // Act
        var result = generator.Generate(1);
        
        // Assert
        await Assert.That(result[0].Split('-')).HasCount(n);
    }
    
    [Test]
    public async Task ReturnsCorrectSeparator_GivenSeparatorOption()
    {
        // Arrange
        var options = new PassphraseOptions { Separator = '>' };
        var generator = new Passphrase(options);
        
        // Act
        var result = generator.Generate(1);
        
        // Assert
        await Assert.That(result[0]).Contains('>').And.DoesNotContain('-');
    }

    [Test]
    public async Task ContainsSingleDigit_WhenDigitPlacementIsOnce()
    {
        // Arrange
        var options = new PassphraseOptions { DigitPlacement = DigitPlacementOption.Once };
        var generator = new Passphrase(options);
        
        // Act
        var result = generator.Generate(1).Single();
        var digitCount = result.Count(char.IsDigit);
        
        // Assert
        await Assert.That(digitCount).IsEqualTo(1);
    }
    
    [Test]
    public async Task DigitQuantityIsEqualToWords_WhenDigitPlacementIsEveryWord()
    {
        // Arrange
        var options = new PassphraseOptions { DigitPlacement = DigitPlacementOption.EveryWord, Words = 4 };
        var generator = new Passphrase(options);
        
        // Act
        var result = generator.Generate(1).Single();
        var digitCount = result.Count(char.IsDigit);
        
        // Assert
        await Assert.That(digitCount).IsEqualTo(4);
    }
    
    [Test]
    public async Task ContainsNoDigits_WhenDigitPlacementIsNone()
    {
        // Arrange
        var options = new PassphraseOptions { DigitPlacement = DigitPlacementOption.None };
        var generator = new Passphrase(options);
        
        // Act
        var result = generator.Generate(1).Single();
        var digitCount = result.Count(char.IsDigit);
        
        // Assert
        await Assert.That(digitCount).IsEqualTo(0);
    }
    
    [Test]
    public async Task ContainsOnlyUppercaseWords_WhenCasingIsUpper()
    {
        // Arrange
        var options = new PassphraseOptions { Casing = CasingOption.Upper };
        var generator = new Passphrase(options);
        var regex = new System.Text.RegularExpressions.Regex("^[A-Z]");

        // Act
        var words = generator.Generate(1).Single().Split('-');

        // Assert
        foreach (var word in words)
        {
            await Assert.That(regex.IsMatch(word)).IsTrue();
        }
    }
    
}
