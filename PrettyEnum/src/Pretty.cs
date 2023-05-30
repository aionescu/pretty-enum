[assembly: System.CLSCompliant(true)]

namespace PrettyEnum {
  using System;

  /// <summary>
  /// Static class that contains methods for parsing pretty-printed enum values.
  /// </summary>
  public static class Pretty {
    /// <summary>
    /// The default separator to use when pretty-printing enums annotated with <see cref="System.FlagsAttribute"/>.
    /// </summary>
    public static string DefaultFlagSeparator { get; } = " | ";

    private static bool TryParseMultiFlags<TEnum>(string prettyName, string flagSeparator, out TEnum result) where TEnum : struct, Enum {
      var flags = prettyName.Split(new[] { flagSeparator }, StringSplitOptions.None);

      var i = 0;
      TEnum tmp = default;
      while (i < flags.Length && PrettyNameCache<TEnum>.singleValueReverseCache.TryGetValue(flags[i], out var flag)) {
        tmp = PrettyNameCache<TEnum>.or(tmp, flag);
        ++i;
      }

      if (i > 0 && i == flags.Length) {
        result = tmp;
        return true;
      } else {
        result = default;
        return false;
      }
    }

    /// <summary>
    /// Returns a span containing the pretty names of all the fields of <typeparamref name="TEnum"/>.
    /// </summary>
    /// <typeparam name="TEnum">The type of the enum.</typeparam>
    public static ReadOnlySpan<string> GetNames<TEnum>() where TEnum : struct, Enum => PrettyNameCache<TEnum>.enumPrettyNames;

    /// <summary>
    /// Attempts to parse the specifed pretty-printed string back into its corresponding enum value.
    /// </summary>
    /// <typeparam name="TEnum">The type of the enum.</typeparam>
    /// <param name="prettyName">The pretty-printed string to parse.</param>
    /// <param name="result">The enum value that corresponds to the specified pretty-printed string, if it exists.</param>
    /// <param name="flagSeparator">The string that was used to separate flags when pretty-printing, in case <typeparamref name="TEnum"/>
    /// is annotated with <see cref="System.FlagsAttribute"/>. Defaults to <see cref="Pretty.DefaultFlagSeparator"/>.</param>
    /// <returns>A boolean value indicating whether parsing was successful.</returns>
    public static bool TryParse<TEnum>(string prettyName, out TEnum result, string flagSeparator = null) where TEnum : struct, Enum {
      if (string.IsNullOrWhiteSpace(prettyName)) {
        result = default;
        return false;
      }

      if (PrettyNameCache<TEnum>.singleValueReverseCache.TryGetValue(prettyName, out result))
        return true;

      if (PrettyNameCache<TEnum>.hasFlagsAttribute)
        return TryParseMultiFlags(prettyName, flagSeparator ?? DefaultFlagSeparator, out result);

      return false;
    }

    /// <summary>
    /// Parses the specifed pretty-printed string back into its corresponding enum value.
    /// </summary>
    /// <typeparam name="TEnum">The type of the enum.</typeparam>
    /// <param name="prettyName">The pretty-printed string to parse.</param>
    /// <param name="flagSeparator">The string that was used to separate flags when pretty-printing, in case <typeparamref name="TEnum"/>
    /// is annotated with <see cref="System.FlagsAttribute"/>. Defaults to <see cref="Pretty.DefaultFlagSeparator"/>.</param>
    /// <returns>The enum value that corresponds to the specified pretty-printed string.</returns>
    /// <exception cref="System.FormatException">Thrown if <paramref name="prettyName"/> is not the pretty name of any enum value.</exception>
    public static TEnum Parse<TEnum>(string prettyName, string flagSeparator = null) where TEnum : struct, Enum {
      if (TryParse<TEnum>(prettyName, out var result, flagSeparator))
        return result;
      else
        throw new FormatException("Input string was not in a correct format.");
    }
  }
}
