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
        }).Throws<ArgumentNullException>();
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
        var options = new PasswordOptions();
        var generator = new Password(options);
        
        // Act
        var result = generator.Generate(n);
        
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
        var options = new PasswordOptions { Length = length };
        var generator = new Password(options);
        
        // Act
        var result = generator.Generate(1).Single();
        
        // Assert
        await Assert.That(result).HasLength().EqualTo(length);

    }
    
    [Test]
    public async Task ContainsNoAmbiguousCharacters_GivenAmbiguousCharsDisabled()
    {
        // Arrange
        var options = new PasswordOptions { IncludeAmbiguousChars = false };
        var generator = new Password(options);
        
        // Act
        var results = generator.Generate(100); // Generate 100 passwords to ensure coverage
        
        // Assert
        foreach (var result in results)
        {
            foreach (var ambiguousChar in Password.AmbiguousChars)
            {
                await Assert.That(result).DoesNotContain(ambiguousChar);
            }
            
        }

    }
    
    [Test]
    public async Task ContainsNoAmbiguousDigits_GivenAmbiguousCharsDisabledAndDigitsEnabled()
    {
        // Arrange
        var options = new PasswordOptions { IncludeAmbiguousChars = false, IncludeDigits = true };
        var generator = new Password(options);
        
        // Act
        var results = generator.Generate(100); // Generate 100 passwords to ensure coverage
        
        // Assert
        foreach (var result in results)
        {
            foreach (var ambiguousDigit in Password.AmbiguousDigits)
            {
                await Assert.That(result).DoesNotContain(ambiguousDigit);
            }
        }
    }
}