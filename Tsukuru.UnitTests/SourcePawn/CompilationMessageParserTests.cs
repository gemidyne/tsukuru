using Tsukuru.SourcePawn;
using Xunit;

namespace Tsukuru.UnitTests.SourcePawn;

public class CompilationMessageParserTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("  ")]
    public void ParseFromString_IgnoresNullOrEmptyCompilerOutput(string input)
    {
        // Arrange

        // Act
        var result = CompilationMessageParser.ParseFromString(input);

        // Assert
        Assert.Null(result);
    }

    [Theory]
    [InlineData("example input 23423")]
    [InlineData("example input 1")]
    [InlineData("example input 432412")]
    [InlineData("example input 523532")]
    [InlineData("example input 6547585")]
    public void ParseFromString_ReturnsNoTypeCompilationMessage_WhenOutputIsNotErrorOrWarning(string input)
    {
        // Arrange
        var expected = new CompilationMessage
        {
            Message = input.Trim(),
            RawLine = input
        };
            
        // Act
        var result = CompilationMessageParser.ParseFromString(input);

        // Assert
        Assert.Equal(expected.Message, result.Message);
        Assert.Equal(expected.RawLine, result.RawLine);
    }
}