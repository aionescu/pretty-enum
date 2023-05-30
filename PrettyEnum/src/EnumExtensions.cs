namespace PrettyEnum {
  using System;
  using System.Linq;

  /// <summary>
  /// Static class that contains pretty-printing extension methods for enum types.
  /// </summary>
  public static class EnumExtensions {
    /// <summary>
    /// Pretty-prints the provided enum value.
    /// </summary>
    /// <typeparam name="TEnum">The type of the enum.</typeparam>
    /// <param name="value">The enum value to pretty-print.</param>
    /// <param name="flagSeparator">The string that should be used to separate flags in case <typeparamref name="TEnum"/>
    /// is annotated with <see cref="System.FlagsAttribute"/>. Defaults to <see cref="Pretty.DefaultFlagSeparator"/>.</param>
    /// <returns></returns>
    /// <exception cref="System.ArgumentException">Thrown when <paramref name="value"/> is not a defined value of <typeparamref name="TEnum"/>.</exception>
    public static string PrettyPrint<TEnum>(this TEnum value, string flagSeparator = null) where TEnum : struct, Enum {
      if (PrettyNameCache<TEnum>.singleValueCache.TryGetValue(value, out var cachedPrettyName))
        return cachedPrettyName;

      if (PrettyNameCache<TEnum>.hasFlagsAttribute) {
        var flags =
          PrettyNameCache<TEnum>.enumValues
          .Where(v => PrettyNameCache<TEnum>.and(value, v))
          .Select(f => PrettyNameCache<TEnum>.singleValueCache[f])
          .ToList();

        flagSeparator ??= Pretty.DefaultFlagSeparator;
        if (flags.Count > 0)
          return string.Join(flagSeparator, flags);
      }

      throw new ArgumentException($"Value {value} is not defined for the enum type {typeof(TEnum).Name}.");
    }
  }
}
