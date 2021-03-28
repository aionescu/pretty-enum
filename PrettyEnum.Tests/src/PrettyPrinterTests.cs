namespace PrettyEnum.Tests {
  using System;
  using Xunit;

  public class PrettyPrinterTests {
    [Fact]
    public void PrettyPrint_FormatsToTitleCase() {
      Assert.Equal("Pascal Case", FormatterTestEnum.PascalCase.PrettyPrint());
      Assert.Equal("Camel Case", FormatterTestEnum.camelCase.PrettyPrint());

      Assert.Equal("Upper Snake Case", FormatterTestEnum.UPPER_SNAKE_CASE.PrettyPrint());
      Assert.Equal("Lower Snake Case", FormatterTestEnum.lower_snake_case.PrettyPrint());

      Assert.Equal("Mixed Snake And Camel Case", FormatterTestEnum.Mixed_SNAKE_And_Camel_case.PrettyPrint());

      Assert.Equal("Capitalized", FormatterTestEnum.Capitalized.PrettyPrint());
      Assert.Equal("Uncapitalized", FormatterTestEnum.uncapitalized.PrettyPrint());

      Assert.Equal("Numbers 123", FormatterTestEnum.Numbers123.PrettyPrint());
      Assert.Equal("Numbers 456 Between", FormatterTestEnum.Numbers456Between.PrettyPrint());
    }

    [Fact]
    public void PrettyPrint_AttributesOverrideDefaultFormatting() {
      Assert.Equal("No Attributes", AttributesTestEnum.NoAttributes.PrettyPrint());

      Assert.Same(AttributesTestEnum.IgnorePrinting.ToString(), AttributesTestEnum.IgnorePrinting.PrettyPrint());

      Assert.Equal("Custom Name", AttributesTestEnum.ExplicitCustomName.PrettyPrint());
      Assert.Equal("Overridden Name", AttributesTestEnum.DescriptionAndName.PrettyPrint());
    }

    [Fact]
    public void PrettyPrint_FlagsHandling() {
      Assert.Equal("Flag 1", FlagsTestEnum.Flag1.PrettyPrint());

      Assert.Equal("Flag 1 | Flag 2", (FlagsTestEnum.Flag1 | FlagsTestEnum.Flag2).PrettyPrint());
      Assert.Equal("Flag 1, Flag 2", (FlagsTestEnum.Flag1 | FlagsTestEnum.Flag2).PrettyPrint(", "));

      Assert.Equal("Flag 2, Flag Eight", (FlagsTestEnum.Flag2 | FlagsTestEnum.Flag8).PrettyPrint(", "));
      Assert.Equal("Flag 1, Flag 4, Flag16", (FlagsTestEnum.Flag16 | FlagsTestEnum.Flag1 | FlagsTestEnum.Flag4).PrettyPrint(", "));
    }

    [Fact]
    public void PrettyPrint_FullyIgnoreFormatting() {
      Assert.Equal(Enum.GetNames<FullyIgnoreFormattingEnum>(), Pretty.GetNames<FullyIgnoreFormattingEnum>());

      var flags = FullyIgnoreFormattingEnum.Ignore1 | FullyIgnoreFormattingEnum.Ignore2 | FullyIgnoreFormattingEnum.Ignore8;
      Assert.Equal(flags.ToString(), flags.PrettyPrint());
    }

    [Fact]
    public void PrettyPrint_UndefinedValues() {
      var undefined = (FormatterTestEnum)(-1);

      Assert.Throws<ArgumentException>(() => undefined.PrettyPrint());
      Assert.Equal("Undefined[-1]", undefined.PrettyPrint(throwOnUndefinedValue: false));
    }
  }
}
