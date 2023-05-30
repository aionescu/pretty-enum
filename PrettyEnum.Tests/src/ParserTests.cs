namespace PrettyEnum.Tests;

using System.Reflection;
using TestEnums;
using Xunit;

public class ParserTests {
  [Fact]
  public void Parse_SingleValues() {
    Assert.Equal(FormatterTestEnum.PascalCase, Pretty.Parse<FormatterTestEnum>("Pascal Case"));
    Assert.Equal(AttributesTestEnum.IgnorePrinting, Pretty.Parse<AttributesTestEnum>("IgnorePrinting"));
    Assert.Equal(AttributesTestEnum.DescriptionAndName, Pretty.Parse<AttributesTestEnum>("Overridden Name"));
  }

  [Fact]
  public void Parse_Flags() {
    Assert.Equal(FlagsTestEnum.Flag1 | FlagsTestEnum.Flag2, Pretty.Parse<FlagsTestEnum>("Flag 1 | Flag 2"));
    Assert.Equal(FlagsTestEnum.Flag1 | FlagsTestEnum.Flag4, Pretty.Parse<FlagsTestEnum>("Flag 1, Flag 4", ", "));
    Assert.Equal(FlagsTestEnum.Flag1 | FlagsTestEnum.Flag8, Pretty.Parse<FlagsTestEnum>("Flag Eight ||| Flag 1", " ||| "));

    Assert.Equal(BindingFlags.Public | BindingFlags.Static, Pretty.Parse<BindingFlags>("Public Static", " "));
  }

  [Fact]
  public void Parse_UndefinedValues() {
    Assert.Throws<FormatException>(() => Pretty.Parse<FlagsTestEnum>("abc"));
    Assert.Throws<FormatException>(() => Pretty.Parse<FlagsTestEnum>(null));
  }

  [Fact]
  public void TryParse_UndefinedValues() {
    Assert.False(Pretty.TryParse<FlagsTestEnum>("abc", out _));
    Assert.False(Pretty.TryParse<FlagsTestEnum>(null, out _));
  }
}
