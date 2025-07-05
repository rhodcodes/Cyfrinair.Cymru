using System.Text.RegularExpressions;

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
            })
            .Throws<ArgumentNullException>();
    }

    [Test]
    public async Task ReturnsSinglePassphrase_GivenOneCount()
    {
        // Arrange
        PassphraseOptions options = new();
        Passphrase generator = new(options);

        // Act
        List<string> result = generator.Generate(1);

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
        PassphraseOptions options = new();
        Passphrase generator = new(options);

        // Act
        List<string> result = generator.Generate(n);

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
        PassphraseOptions options = new() { Words = n };
        Passphrase generator = new(options);

        // Act
        List<string> result = generator.Generate(1);

        // Assert
        await Assert.That(result[0].Split('-')).HasCount(n);
    }

    [Test]
    public async Task ReturnsCorrectSeparator_GivenSeparatorOption()
    {
        // Arrange
        PassphraseOptions options = new() { Separator = '>' };
        Passphrase generator = new(options);

        // Act
        List<string> result = generator.Generate(1);

        // Assert
        await Assert.That(result[0]).Contains('>').And.DoesNotContain('-');
    }

    [Test]
    public async Task ContainsSingleDigit_WhenDigitPlacementIsOnce()
    {
        // Arrange
        PassphraseOptions options = new() { DigitPlacement = DigitPlacementOption.Once };
        Passphrase generator = new(options);

        // Act
        string result = generator.Generate(1).Single();
        int digitCount = result.Count(char.IsDigit);

        // Assert
        await Assert.That(digitCount).IsEqualTo(1);
    }

    [Test]
    public async Task DigitQuantityIsEqualToWords_WhenDigitPlacementIsEveryWord()
    {
        // Arrange
        PassphraseOptions options = new() { DigitPlacement = DigitPlacementOption.EveryWord, Words = 4 };
        Passphrase generator = new(options);

        // Act
        string result = generator.Generate(1).Single();
        int digitCount = result.Count(char.IsDigit);

        // Assert
        await Assert.That(digitCount).IsEqualTo(4);
    }

    [Test]
    public async Task ContainsNoDigits_WhenDigitPlacementIsNone()
    {
        // Arrange
        PassphraseOptions options = new() { DigitPlacement = DigitPlacementOption.None };
        Passphrase generator = new(options);

        // Act
        string result = generator.Generate(1).Single();
        int digitCount = result.Count(char.IsDigit);

        // Assert
        await Assert.That(digitCount).IsEqualTo(0);
    }

    [Test]
    public async Task ContainsOnlyUppercaseWords_WhenCasingIsUpper()
    {
        // Arrange
        PassphraseOptions options = new() { Casing = CasingOption.Upper };
        Passphrase generator = new(options);
        Regex regex = new("^[A-Z]");

        // Act
        string[] words = generator.Generate(1).Single().Split('-');

        // Assert
        foreach (string word in words)
        {
            await Assert.That(regex.IsMatch(word)).IsTrue();
        }
    }

    [Test]
    public async Task ContainsOnlyLowercaseWords_WhenCasingIsLower()
    {
        // Arrange
        PassphraseOptions options = new() { Casing = CasingOption.Lower };
        Passphrase generator = new(options);
        Regex regex = new("^[a-z]");

        // Act
        string[] words = generator.Generate(1).Single().Split('-');

        // Assert
        foreach (string word in words)
        {
            await Assert.That(regex.IsMatch(word)).IsTrue();
        }
    }

    [Test]
    public async Task ContainsMixedCaseWords_WhenCasingIsRandom()
    {
        // Arrange
        PassphraseOptions options = new() { Casing = CasingOption.Random };
        Passphrase generator = new(options);

        // Act
        string[] words = generator.Generate(1).Single().Split('-');

        // Assert
        bool hasUpper = words.Any(word => word.Any(char.IsUpper));
        bool hasLower = words.Any(word => word.Any(char.IsLower));

        await Assert.That(hasUpper).IsTrue();
        await Assert.That(hasLower).IsTrue();
    }

    [Test]
    public async Task ReturnsCorrectSeparator_GivenWhitespaceSeparator()
    {
        // Arrange
        PassphraseOptions options = new() { Separator = ' ' };
        Passphrase generator = new(options);

        // Act
        List<string> result = generator.Generate(1);

        // Assert
        await Assert.That(result[0]).Contains(' ').And.DoesNotContain('-');
    }

    [Test]
    public async Task ReturnsCorrectSeparator_GivenSpecialCharacterSeparator()
    {
        // Arrange
        PassphraseOptions options = new() { Separator = '_' };
        Passphrase generator = new(options);

        // Act
        List<string> result = generator.Generate(1);

        // Assert
        await Assert.That(result[0]).Contains('_').And.DoesNotContain('-');
    }

    [Test]
    public async Task DigitQuantityIsEqualToWords_WhenDigitPlacementIsEveryWordAndCasingIsLower()
    {
        // Arrange
        PassphraseOptions options = new()
        {
            DigitPlacement = DigitPlacementOption.EveryWord, Words = 3, Casing = CasingOption.Lower
        };
        Passphrase generator = new(options);

        // Act
        string result = generator.Generate(1).Single();
        int digitCount = result.Count(char.IsDigit);

        // Assert
        await Assert.That(digitCount).IsEqualTo(3);
    }

    [Test]
    public async Task ContainsSingleDigit_WhenDigitPlacementIsOnceAndCasingIsRandom()
    {
        // Arrange
        PassphraseOptions options = new() { DigitPlacement = DigitPlacementOption.Once, Casing = CasingOption.Random };
        Passphrase generator = new(options);

        // Act
        string result = generator.Generate(1).Single();
        int digitCount = result.Count(char.IsDigit);

        // Assert
        await Assert.That(digitCount).IsEqualTo(1);
    }

    [Test]
    public async Task ThrowsArgumentException_GivenZeroWords()
    {
        // Arrange
        PassphraseOptions options = new() { Words = 0 };
        Passphrase generator = new(options);

        // Act & Assert
        await Assert.That(() => generator.Generate(1))
            .Throws<ArgumentException>();
    }

    [Test]
    public async Task ThrowsArgumentException_GivenNegativeWords()
    {
        // Arrange
        PassphraseOptions options = new() { Words = -1 };
        Passphrase generator = new(options);

        // Act & Assert
        await Assert.That(() => generator.Generate(1))
            .Throws<ArgumentException>();
    }

    [Test]
    public async Task ThrowsArgumentException_GivenInvalidSeparator()
    {
        // Arrange
        PassphraseOptions options = new() { Separator = '\0' };
        Passphrase generator = new(options);

        // Act & Assert
        await Assert.That(() => generator.Generate(1))
            .Throws<ArgumentException>();
    }

    [Test]
    public async Task ReturnsPassphrase_GivenLargeWordCount()
    {
        // Arrange
        PassphraseOptions options = new() { Words = 1000 };
        Passphrase generator = new(options);

        // Act
        List<string> result = generator.Generate(1);

        // Assert
        await Assert.That(result[0].Split('-')).HasCount(1000);
    }

    [Test]
    public async Task ReturnsMultipleDistinctPassphrases_GivenLargeQuantity()
    {
        // Arrange
        PassphraseOptions options = new();
        Passphrase generator = new(options);

        // Act
        List<string> result = generator.Generate(ushort.MaxValue);

        // Assert
        await Assert.That(result).HasCount(ushort.MaxValue).And.HasDistinctItems();
    }


    [Test]
    public async Task GeneratesPassphrasesSafely_WhenUsedInParallel()
    {
        // Arrange
        PassphraseOptions options = new();
        Passphrase generator = new(options);

        // Act
        List<string>[] results =
            await Task.WhenAll(Enumerable.Range(0, 100).Select(_ => Task.Run(() => generator.Generate(1))));

        // Assert
        await Assert.That(results.SelectMany(r => r)).HasDistinctItems();
    }
}