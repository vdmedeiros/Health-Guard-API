using Xunit;
using HealthcareApi.Application.Validators;

namespace HealthcareApi.Tests;

public class CpfValidatorTests
{
    [Theory]
    [InlineData("529.982.247-25", true)]
    [InlineData("52998224725", true)]
    [InlineData("123.456.789-09", true)]
    [InlineData("11111111111", false)]
    [InlineData("00000000000", false)]
    [InlineData("12345678901", false)]
    [InlineData("123456789", false)]
    [InlineData("", false)]
    [InlineData(null, false)]
    public void IsValid_ShouldValidateCpfCorrectly(string? cpf, bool expected)
    {
        var result = CpfValidator.IsValid(cpf!);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("529.982.247-25", "52998224725")]
    [InlineData("52998224725", "52998224725")]
    [InlineData("123.456.789-09", "12345678909")]
    public void Normalize_ShouldRemoveFormatting(string cpf, string expected)
    {
        var result = CpfValidator.Normalize(cpf);
        Assert.Equal(expected, result);
    }

    [Theory]
    [InlineData("52998224725", "529.982.247-25")]
    [InlineData("12345678909", "123.456.789-09")]
    public void Format_ShouldAddFormatting(string cpf, string expected)
    {
        var result = CpfValidator.Format(cpf);
        Assert.Equal(expected, result);
    }
}
