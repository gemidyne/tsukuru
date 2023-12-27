using Tsukuru.Converters;
using Xunit;

namespace Tsukuru.UnitTests.Converters;

public class BooleanToPackingModeConverterTests
{
    [Theory]
    [InlineData(0, "All files")]
    [InlineData(null, "All files")]
    [InlineData(false, "All files")]
    [InlineData(true, "Only files used by the map")]
    public void Convert_WhenProvidedInput_ReturnsExpected(
        object input,
        string expected)
    {
        // Arrange
        var unitUnderTest = new BooleanToPackingModeConverter();

        // Act
        var actual = unitUnderTest.Convert(input, null, null, null);

        // Assert
        Assert.Equal(expected, actual);
    }
    
    [Theory]
    [InlineData("Only files used by the map", true)]
    [InlineData("Anything else", false)]
    public void ConvertBack_WhenProvidedInput_ReturnsExpected(
        object input,
        bool expected)
    {
        // Arrange
        var unitUnderTest = new BooleanToPackingModeConverter();

        // Act
        var actual = unitUnderTest.ConvertBack(input, null, null, null);

        // Assert
        Assert.Equal(expected, actual);
    }
}