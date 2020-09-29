namespace PrettyEnum {
  using System;
  using System.Linq;

  public static class Pretty {
    public static readonly string DefaultFlagSeparator = " | ";

    public static T Parse<T>(string prettyName, string flagSeparator = null) where T : struct, Enum =>
      TryParse<T>(prettyName, out var result, flagSeparator)
      ? result
      : throw new FormatException("Input string was not in a correct format.");

    public static bool TryParse<T>(string prettyName, out T result, string flagSeparator = null) where T : struct, Enum {
      PrettyNameCache<T>._populateSingleValueCache();

      if (_tryParseSingleValue(prettyName, out result))
        return true;

      if (typeof(T)._hasAttribute<FlagsAttribute>())
        return _tryParseFlags(prettyName, flagSeparator ?? DefaultFlagSeparator, out result);
      else
        return false;
    }

    private static bool _tryParseFlags<T>(string value, string flagSeparator, out T result) where T : struct, Enum {
      var flagsMatch = PrettyNameCache<T>._multiFlagsCache.FirstOrDefault(kvp => kvp.Key.FlagSeparator == flagSeparator && kvp.Value == value);

      if (flagsMatch.Value != null) {
        result = flagsMatch.Key.EnumValue;
        return true;
      }

      try {
        var flags = value.Split(new[] { flagSeparator }, StringSplitOptions.None).Select(_parseSingleValue<T>);
        return Enum.TryParse(string.Join(",", flags), out result);
      } catch {
        result = default;
        return false;
      }
    }

    private static T _parseSingleValue<T>(string value) where T : struct, Enum =>
      _tryParseSingleValue<T>(value, out var result)
      ? result
      : throw new FormatException("Input string was not in a correct format.");

    private static bool _tryParseSingleValue<T>(string value, out T result) where T : struct, Enum {
      var match = PrettyNameCache<T>._singleValueCache.FirstOrDefault(kvp => kvp.Value == value);

      if (match.Value is null)
        return Enum.TryParse(value, out result);
      else {
        result = match.Key;
        return true;
      }
    }
  }
}
