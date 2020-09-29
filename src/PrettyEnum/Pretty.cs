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
      _populateCache<T>();

      if (_tryParseNoFlags(prettyName, out result))
        return true;

      if (typeof(T).GetCustomAttributes(typeof(FlagsAttribute), false).Length > 0)
        return _tryParseFlags(prettyName, flagSeparator ?? DefaultFlagSeparator, out result);
      else
        return false;
    }

    private static void _populateCache<T>() where T : struct, Enum {
      var cache = PrettyNameCache<T>._cache;

      foreach (var value in Enum.GetValues(typeof(T)).Cast<T>())
        if (!cache.ContainsKey(value))
          value.PrettyPrint();
    }

    private static bool _tryParseFlags<T>(string value, string flagSeparator, out T result) where T : struct, Enum {
      var flagsMatch = PrettyNameCache<T>._flagsCache.FirstOrDefault(kvp => kvp.Key.FlagSeparator == flagSeparator && kvp.Value == value);

      if (flagsMatch.Value != null) {
        result = flagsMatch.Key.EnumValue;
        return true;
      }

      try {
        var flags = value.Split(new[] { flagSeparator }, StringSplitOptions.None).Select(_parseNoFlags<T>);
        var enumValue = (T)Enum.Parse(typeof(T), string.Join(",", flags));

        result = (T)Enum.Parse(typeof(T), string.Join(",", flags));
        return true;
      } catch {
        result = default;
        return false;
      }
    }

    private static T _parseNoFlags<T>(string value) where T : struct, Enum {
      return _tryParseNoFlags<T>(value, out var result) ? result : throw new FormatException("Input string was not in a correct format.");
    }

    private static bool _tryParseNoFlags<T>(string value, out T result) where T : struct, Enum {
      var match = PrettyNameCache<T>._cache.FirstOrDefault(kvp => kvp.Value == value);

      if (match.Value is null)
        return Enum.TryParse(value, out result);
      else {
        result = match.Key;
        return true;
      }
    }
  }
}
