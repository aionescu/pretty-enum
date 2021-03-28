namespace PrettyEnum.Tests {
  using System;
  using System.ComponentModel;

  enum FormatterTestEnum {
    PascalCase,
    camelCase,
    UPPER_SNAKE_CASE,
    lower_snake_case,
    Mixed_SNAKE_And_Camel_case,
    Capitalized,
    uncapitalized,
    Numbers123,
    Numbers456Between
  }

  enum AttributesTestEnum: short {
    NoAttributes,

    [IgnorePrettyPrint]
    IgnorePrinting,

    [PrettyName("Custom Name")]
    ExplicitCustomName,

    [Description("Descritpion Name"), PrettyName("Overridden Name")]
    DescriptionAndName,

    [PreserveCase]
    Explicit_preserveCase_ATTRIBUTE
  }

  [Flags]
  enum FlagsTestEnum: byte {
    Flag1 = 1,
    Flag2 = 2,
    Flag4 = 4,

    [PrettyName("Flag Eight")]
    Flag8 = 8,

    [IgnorePrettyPrint]
    Flag16 = 16
  }

  [Flags, IgnorePrettyPrint]
  enum FullyIgnoreFormattingEnum {
    Ignore1 = 1,
    Ignore2 = 2,
    Ignore4 = 4,
    Ignore8 = 8
  }
}
