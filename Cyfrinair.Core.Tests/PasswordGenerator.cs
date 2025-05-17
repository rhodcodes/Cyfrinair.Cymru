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
    public async Task ReturnsSinglePassword_GivenOneCount()
    {
        // Arrange
        var options = new PasswordOptions();
        var generator = new Password(options);
        
        // Act
        var result = generator.Generate(1);
        
        // Assert
        await Assert.That(result).HasSingleItem();

    }
    
    [Test]
    public async Task ReturnsMultiplePasswords_GivenMultipleCount()
    {
        // Arrange
        var options = new PasswordOptions();
        var generator = new Password(options);
        
        // Act
        var result = generator.Generate(5);
        
        // Assert
        await Assert.That(result).HasCount(5);
    }
    
    [Test]
    public async Task ReturnsDistinctPasswords_GivenMultipleCount()
    {
        // Arrange
        var options = new PasswordOptions();
        var generator = new Password(options);
        
        // Act
        var result = generator.Generate(5);
        
        // Assert
        await Assert.That(result).HasDistinctItems();
    }
    
    [Test]
    public async Task ReturnsPasswordWithCorrectLength_GivenValidLength()
    {
        // Arrange
        var options = new PasswordOptions { Length = 12 };
        var generator = new Password(options);
        
        // Act
        var result = generator.Generate(1);
        
        // Assert
        await Assert.That(result[0]).HasLength().EqualTo(12);

    }
}