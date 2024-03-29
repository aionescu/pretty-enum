# pretty-enum

[![NuGet](https://img.shields.io/nuget/v/pretty-enum?style=for-the-badge)](https://nuget.org/packages/pretty-enum)

A .NET Standard library for pretty-printing enum values

## Usage

`pretty-enum` aims to be as easy to use out-of-the-box as possible.

All you need for the examples below is `using PrettyEnum;`.

To use the default pretty-printer to format an enum value, just use `enum.PrettyPrint()`. It will automatically format `PascalCase`, `camelCase`, `Snake_Case`, and `UPPER_SNAKE_CASE` (as well as `MixedVersions_OfThem`) to `Title Case`.

Examples:

```cs
StringSplitOptions.RemoveEmptyEntries.PrettyPrint() == "Remove Empty Entries"
SomeEnum.UPPER_SNAKE_CASE.PrettyPrint() == "Upper Snake Case"
SomeEnum.MixedCamel_AndSnake.PrettyPrint() == "Mixed Camel And Snake"
```

To specify a custom value to be used when pretty-printng, annotate the enum field with `PrettyNameAttribute`, like so:

```cs
using PrettyEnum;

public enum Color {
  Red,
  DarkRed,
  [PrettyName("Even Darker Red")]
  ReallyDarkRed
}

// PrettyPrint() will use the value specified in the attribute:
Color.ReallyDarkRed.PrettyPrint() == "Even Darker Red"
```

PrettyEnum also recognizes `DescriptionAttribute` from `System.ComponentModel` (although `PrettyNameAttribute` takes precedence if both are present).

You can also annotate either enum fields or whole enum types with the `IgnorePrettyPrintAttribute`, in which case the pretty-printer will just use the name of the enum field(s).

It's also possible to get a `ReadOnlySpan` containing all the pretty names of a particular enum type:

```cs
Pretty.GetNames<DeliveryOptions>() == ["Same Day", "Fragile", "Contactless"]
Pretty.GetNames<Color>() == ["Red", "Dark Red", "Even Darker Red"]
```

### Flag-like Enums

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

(DeliveryOptions.SameDay | DeliveryOptions.ExtraPackaging).PrettyPrint() == "Same Day | Fragile"
(DeliveryOptions.SameDay | DeliveryOptions.Contactless).PrettyPrint(", ") == "Same Day, Contactless"
```

### Parsing

`pretty-enum` also supports parsing pretty-printed strings back into their corresponding enum values, including values containing multiple flags.

Two methods are provided for this purpse: `Pretty.TryParse` (which returns a `bool` indicating success or failure, and writes the parsed enum value to an `out` parameter), and `Pretty.Parse` (which directly returns the parsed enum value, or throws an exception on failure).

Example:

```cs
Pretty.Parse<DeliveryOptions>("Same Day | Fragile") == (DeliveryOptions.SameDay | DeliveryOptions.ExtraPackaging)
Pretty.Parse<BindingFlags>("Static & Public", " & ") == (BindingFlags.Public | BindingFlags.Static)
Pretty.TryParse<DeliveryOptions>("Same Day, Contactless", out var value, flagSeparator: ", ") // returns true, value == (DeliveryOptions.SameDay | DeliveryOptions.Contactless)
```

## License

This repository is licensed under the terms of the MIT License.
For more details, see [the license file](LICENSE.txt).
