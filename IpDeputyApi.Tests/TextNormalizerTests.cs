using IpDeputyApi.Services;

namespace IpDeputyApi.Tests;

public class TextNormalizerTests
{
    [Theory]
    [InlineData("  Олег   Іванович ", "Олег Іванович")]
    [InlineData("Сидоренко\t\tСтарший", "Сидоренко Старший")]
    [InlineData(null, "")]
    [InlineData("   ", "")]
    public void Clean_TrimsAndCollapsesWhitespace(string? input, string expected)
    {
        Assert.Equal(expected, TextNormalizer.Clean(input));
    }

    [Theory]
    [InlineData("   ", null)]
    [InlineData(null, null)]
    [InlineData(" a  b ", "a b")]
    public void CleanOptional_ReturnsNullForEmpty(string? input, string? expected)
    {
        Assert.Equal(expected, TextNormalizer.CleanOptional(input));
    }
}
