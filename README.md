# pretty-enum

[![NuGet](https://img.shields.io/nuget/v/pretty-enum?style=for-the-badge)](https://nuget.org/packages/pretty-enum)

A .NET Standard library for pretty-printing enum values

## Usage

`pretty-enum` aims to be as easy to use out-of-the-box as possible.

All you need for the examples below is `using PrettyEnum;`.

To use the default pretty-printer to format an enum value, just use `enum.PrettyPrint()`. It will automatically format `PascalCase`, `camelCase`, `Snake_case`, and `UPPER_SNAKE_CASE` enums to `Title Case`.

To specify a custom value to be used when pretty-printng, annotate the enum field with `PrettyNameAttribute`, like so:

```cs
using PrettyEnum;

public enum Color {
  Red,
  DarkRed,
  [PrettyName("Even Darker Red")]
  ReallyDarkRed
}
```

PrettyEnum also recognizes `DescriptionAttribute` from `System.ComponentModel` (although `PrettyNameAttribute` takes precedence if both are present).

You can also annotate either enum fields or whole enum types with the `IgnorePrettyPrintAttribute`, in which case the pretty-printer will just use the name of the enum field(s).

### Flags

The library can also handle flag-like enums (i.e. enums annotated wtih `FlagsAttribute`).

Each individual field is formatted using the above-described rules, and values that contain multiple flags are formatted by joining the names of each flag with a user-provided separator (or, by default, `|`).

Example:

```cs
[Flags]
enum DeliveryOptions {
  SameDay,
  [PrettyName("Fragile")]
  ExtraPackaging,
  Contactless
}

(DeliveryOptions.SameDay | DeliveryOptions.ExtraPackaging).PrettyPrint() // returns "Same Day | Fragile"
(DeliveryOptions.SameDay | DeliveryOptions.Contactless).PrettyPrint(", ") // returns "Same Day, Contactless"
```

### Parsing

`pretty-enum` also supports parsing pretty-printed strings back into their corresponding enum values, including values containing multiple flags.

Two methods are provided for this purpse: `Pretty.Parse` and `Pretty.TryParse`.

Example:

```cs
Pretty.Parse<DeliveryOptions>("Same Day | Fragile") // == (DeliveryOptions.SameDay | DeliveryOptions.ExtraPackaging)
Pretty.TryParse<DeliveryOptions>("Same Day, Contactless", out var value, flagSeparator: ", ") // returns true, value == (DeliveryOptions.SameDay | DeliveryOptions.Contactless)
```

## License

This repository is licensed under the terms of the MIT License.
For more details, see [the license file](LICENSE.txt).
