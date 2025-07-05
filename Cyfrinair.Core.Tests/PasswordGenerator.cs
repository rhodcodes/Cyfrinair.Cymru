using TUnit.Assertions.AssertConditions.Throws;

namespace Cyfrinair.Core.Tests;

public class PasswordGenerator
{
    [Test]
    public async Task ThrowsArgumentNullException_GivenNullOption()
    {
        // Arrange
        PasswordOptions options = null;

        // Act & Assert
        await Assert.That(() =>
            {
                new Password(options);
            })
            .Throws<ArgumentNullException>();
    }

    [Test]
    [Arguments(1)]
    [Arguments(5)]
    [Arguments(10)]
    [Arguments(20)]
    [Arguments(50)]
    [Arguments(100)]
    public async Task ReturnsNDistinctPasswords_GivenNCount(ushort n)
    {
        // Arrange
        PasswordOptions options = new();
        Password generator = new(options);

        // Act
        List<string> result = generator.Generate(n);

        // Assert
        await Assert.That(result).HasCount(n).And.HasDistinctItems();
    }

    [Test]
    [Arguments(10)]
    [Arguments(15)]
    [Arguments(50)]
    [Arguments(100)]
    public async Task ReturnsPasswordWithCorrectLength_GivenValidLength(int length)
    {
        // Arrange
        PasswordOptions options = new() { Length = length };
        Password generator = new(options);

        // Act
        string result = generator.Generate(1).Single();

        // Assert
        await Assert.That(result).HasLength().EqualTo(length);
    }

    [Test]
    public async Task ContainsNoAmbiguousCharacters_GivenAmbiguousCharsDisabled()
    {
        // Arrange
        PasswordOptions options = new() { IncludeAmbiguousChars = false };
        Password generator = new(options);

        // Act
        List<string> results = generator.Generate(100); // Generate 100 passwords to ensure coverage

        // Assert
        foreach (string result in results)
        {
            foreach (char ambiguousChar in Password.AmbiguousChars)
            {
                await Assert.That(result).DoesNotContain(ambiguousChar);
            }
        }
    }

    [Test]
    public async Task ContainsNoAmbiguousDigits_GivenAmbiguousCharsDisabledAndDigitsEnabled()
    {
        // Arrange
        PasswordOptions options = new() { IncludeAmbiguousChars = false, IncludeDigits = true };
        Password generator = new(options);

        // Act
        List<string> results = generator.Generate(100); // Generate 100 passwords to ensure coverage

        // Assert
        foreach (string result in results)
        {
            foreach (char ambiguousDigit in Password.AmbiguousDigits)
            {
                await Assert.That(result).DoesNotContain(ambiguousDigit);
            }
        }
    }

    [Test]
    public async Task ContainsNoAmbiguousSymbols_GivenSymbolsEnabled()
    {
        // Arrange
        PasswordOptions options = new() { IncludeSymbols = true, IncludeAmbiguousChars = false };
        Password generator = new(options);

        // Act
        List<string> results = generator.Generate(100); // Generate 100 passwords to ensure coverage

        // Assert

        foreach (string result in results)
        {
            foreach (char ambiguousSymbol in Password.AmbiguousSymbols)
            {
                await Assert.That(result).DoesNotContain(ambiguousSymbol);
            }
        }
    }

    [Test]
    public async Task ContainsAllAmbiguousChars_WhenAllOptionsEnabled()
    {
        PasswordOptions options = new() { IncludeDigits = true, IncludeSymbols = true, IncludeAmbiguousChars = true };
        Password generator = new(options);

        List<string> results = generator.Generate(100);

        bool foundAmbiguousChar = results.Any(pw => Password.AmbiguousChars.Any(pw.Contains));
        bool foundAmbiguousSymbol = results.Any(pw => Password.AmbiguousSymbols.Any(pw.Contains));
        bool foundAmbiguousDigit = results.Any(pw => Password.AmbiguousDigits.Any(pw.Contains));

        await Assert.That(foundAmbiguousChar).IsTrue();
        await Assert.That(foundAmbiguousSymbol).IsTrue();
        await Assert.That(foundAmbiguousDigit).IsTrue();
    }

    [Test]
    public async Task ContainsOnlyAmbiguousChars_WhenOnlyAmbiguousCharsEnabled()
    {
        PasswordOptions options = new() { IncludeDigits = false, IncludeSymbols = false, IncludeAmbiguousChars = true };
        Password generator = new(options);

        List<string> results = generator.Generate(100);

        foreach (string result in results)
        {
            foreach (char c in result)
            {
                await Assert.That(
                        Password.BaseCharacterSet.Contains(c) ||
                        Password.AmbiguousChars.Contains(c)
                    )
                    .IsTrue();
            }
        }
    }

    [Test]
    public async Task ContainsOnlySymbols_WhenOnlySymbolsEnabled()
    {
        PasswordOptions options = new() { IncludeDigits = false, IncludeSymbols = true, IncludeAmbiguousChars = false };
        Password generator = new(options);

        List<string> results = generator.Generate(100);

        foreach (string result in results)
        {
            foreach (char c in result)
            {
                await Assert.That(
                        Password.BaseCharacterSet.Contains(c) ||
                        Password.SymbolSet.Contains(c)
                    )
                    .IsTrue();
            }
        }
    }

    [Test]
    public async Task ContainsOnlyDigits_WhenOnlyDigitsEnabled()
    {
        PasswordOptions options = new() { IncludeDigits = true, IncludeSymbols = false, IncludeAmbiguousChars = false };
        Password generator = new(options);

        List<string> results = generator.Generate(100);

        foreach (string result in results)
        {
            foreach (char c in result)
            {
                await Assert.That(
                        Password.BaseCharacterSet.Contains(c) ||
                        Password.DigitSet.Contains(c)
                    )
                    .IsTrue();
            }
        }
    }

    [Test]
    public async Task ContainsAmbiguousSymbolsButNoDigits_WhenSymbolsAndAmbiguousCharsEnabled()
    {
        PasswordOptions options = new() { IncludeDigits = false, IncludeSymbols = true, IncludeAmbiguousChars = true };
        Password generator = new(options);

        List<string> results = generator.Generate(100);

        bool foundAmbiguousSymbol = results.Any(pw => Password.AmbiguousSymbols.Any(pw.Contains));
        bool foundAmbiguousDigit = results.Any(pw => Password.AmbiguousDigits.Any(pw.Contains));

        await Assert.That(foundAmbiguousSymbol).IsTrue();
        await Assert.That(foundAmbiguousDigit).IsFalse();
    }

    [Test]
    public async Task ContainsAmbiguousDigitsButNoSymbols_WhenDigitsAndAmbiguousCharsEnabled()
    {
        PasswordOptions options = new() { IncludeDigits = true, IncludeSymbols = false, IncludeAmbiguousChars = true };
        Password generator = new(options);

        List<string> results = generator.Generate(100);

        bool foundAmbiguousDigit = results.Any(pw => Password.AmbiguousDigits.Any(pw.Contains));
        bool foundAmbiguousSymbol = results.Any(pw => Password.AmbiguousSymbols.Any(pw.Contains));

        await Assert.That(foundAmbiguousDigit).IsTrue();
        await Assert.That(foundAmbiguousSymbol).IsFalse();
    }

    [Test]
    public async Task ContainsNoDigitsOrSymbolsOrAmbiguous_WhenAllOptionsDisabled()
    {
        PasswordOptions options =
            new() { IncludeDigits = false, IncludeSymbols = false, IncludeAmbiguousChars = false };
        Password generator = new(options);

        List<string> results = generator.Generate(100);

        foreach (string result in results)
        {
            foreach (char c in result)
            {
                await Assert.That(Password.BaseCharacterSet.Contains(c)).IsTrue();
            }
        }
    }

    [Test]
    public async Task ThrowsArgumentException_GivenZeroLength()
    {
        PasswordOptions options = new() { Length = 0 };
        await Assert.That(() => new Password(options).Generate(1))
            .Throws<ArgumentException>();
    }

    [Test]
    public async Task ThrowsArgumentException_GivenNegativeLength()
    {
        PasswordOptions options = new() { Length = -5 };
        await Assert.That(() => new Password(options).Generate(1))
            .Throws<ArgumentException>();
    }

    [Test]
    public async Task ReturnsPasswordWithMaxLength_GivenLargeLength()
    {
        // Adjust max length as appropriate for your application
        int maxLength = 1000;
        PasswordOptions options = new() { Length = maxLength };
        Password generator = new(options);

        string result = generator.Generate(1).Single();

        await Assert.That(result).HasLength().EqualTo(maxLength);
    }

    [Test]
    public async Task ReturnsAllPasswords_GivenLargeQuantity()
    {
        ushort quantity = 1000; // Use a large but reasonable value for test speed
        PasswordOptions options = new();
        Password generator = new(options);

        List<string> results = generator.Generate(quantity);

        await Assert.That(results).HasCount(quantity).And.HasDistinctItems();
    }

    [Test]
    public async Task ReturnsPasswordWithBaseSet_GivenAllOptionsDisabledAndLengthGreaterThanZero()
    {
        PasswordOptions options = new()
        {
            IncludeDigits = false, IncludeSymbols = false, IncludeAmbiguousChars = false, Length = 10
        };
        Password generator = new(options);

        string result = generator.Generate(1).Single();

        foreach (char c in result)
        {
            await Assert.That(Password.BaseCharacterSet.Contains(c)).IsTrue();
        }
    }
}