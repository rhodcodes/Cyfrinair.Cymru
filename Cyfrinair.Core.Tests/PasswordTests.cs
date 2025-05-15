using TUnit.Assertions.AssertConditions.Throws;

namespace Cyfrinair.Core.Tests;

public class PasswordTests
{
    [Test]
    public async Task PassingNullOptions_ThrowsArgumentNullException()
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
    public async Task GenerateSinglePassword_ReturnsSinglePassword()
    {
        // Arrange
        var options = new PasswordOptions();
        var generator = new Password(options);
        
        // Act
        var result = generator.Generate(1);
        
        // Assert
        await Assert.That(result).HasSingleItem();

    }
}