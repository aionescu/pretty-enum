namespace PrettyEnum {
  using System;
  using System.ComponentModel;
  using System.Linq;
  using System.Reflection;
  using System.Text.RegularExpressions;

  /// <summary>
  /// Static class that contains pretty-printing extension methods for enum types.
  /// </summary>
  public static class EnumExtensions {
    internal static bool _hasAttribute<TAttribute>(this MemberInfo @this, out TAttribute attribute) where TAttribute : Attribute {
      var attrs = @this.GetCustomAttributes(typeof(TAttribute), false) as TAttribute[];

      if (attrs.Length > 0) {
        attribute = attrs[0];
        return true;
      } else {
        attribute = default;
        return false;
      }
    }

    internal static bool _hasAttribute<TAttribute>(this MemberInfo @this) where TAttribute : Attribute => @this._hasAttribute<TAttribute>(out _);

    private static string _regexReplace(this string @this, string pattern, string replacement) => Regex.Replace(@this, pattern, replacement, RegexOptions.Compiled);

    private static string _capitalize(string s) =>
      char.IsLetter(s[0])
      ? char.ToUpperInvariant(s[0]) + s.Substring(1).ToLowerInvariant()
      : s;

    private static string _fromUndefined<T>(T value, bool throwOnUndefinedValue) where T : struct, Enum =>
      throwOnUndefinedValue
      ? throw new ArgumentException($"Value {value} is not defined for the enum type {typeof(T).Name}.")
      : $"Undefined[{value}]";

    private static string _fromCamelCase(string s, bool preserveCase) {
      var replaced =
        s
        ._regexReplace("([A-Z])([a-z])", " $1$2")
        ._regexReplace("([a-z])([A-Z])", "$1 $2")
        ._regexReplace("([0-9])([a-z]|[A-Z])", " $1$2")
        ._regexReplace("([a-z]|[A-Z])([0-9])", "$1 $2");

      var words = replaced.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

      return string.Join(" ", preserveCase ? words : words.Select(_capitalize));
    }

    internal static string _fromSingleValue<T>(T value, bool throwOnUndefinedValue) where T : struct, Enum {
      var rawName = value.ToString();
      var field = typeof(T).GetField(rawName);

      if (typeof(T)._hasAttribute<IgnorePrettyPrintAttribute>() || field._hasAttribute<IgnorePrettyPrintAttribute>())
        return rawName;

      if (field._hasAttribute<PrettyNameAttribute>(out var prettyNameAttr))
        return
          string.IsNullOrWhiteSpace(prettyNameAttr.PrettyName)
          ? rawName
          : prettyNameAttr.PrettyName;

      if (field._hasAttribute<DescriptionAttribute>(out var descAttr))
        return
          string.IsNullOrWhiteSpace(descAttr.Description)
          ? rawName
          : descAttr.Description;

      var preserveCase = typeof(T)._hasAttribute<PreserveCaseAttribute>() || field._hasAttribute<PreserveCaseAttribute>();

      var words =
        rawName
        .Split(new[] { '_' }, StringSplitOptions.RemoveEmptyEntries)
        .Select(s => _fromCamelCase(s, preserveCase));

      return string.Join(" ", words);
    }

    private static string _fromMultiFlags<T>(T value, string flagSeparator, bool throwOnUndefinedValue) where T : struct, Enum {
      if (PrettyNameCache<T>._multiFlagsCache.TryGetValue((value, flagSeparator), out var cachedName))
        return cachedName;

      var flags = PrettyNameCache<T>._enumValues.Where(f => value.HasFlag(f)).Select(f => _fromSingleValue(f, false));

      if (!flags.Any())
        return _fromUndefined(value, throwOnUndefinedValue);

      var prettyName = string.Join(flagSeparator, flags);
      PrettyNameCache<T>._multiFlagsCache[(value, flagSeparator)] = prettyName;

      return prettyName;
    }

    /// <summary>
    /// Pretty-prints the provided enum value.
    /// </summary>
    /// <typeparam name="T">The type of the enum.</typeparam>
    /// <param name="enumValue">The enum value to pretty-print.</param>
    /// <param name="flagSeparator">The string that should be used to separate flags in case <typeparamref name="T"/>
    /// is annotated with <see cref="System.FlagsAttribute"/>. Defaults to <see cref="Pretty.DefaultFlagSeparator"/>.</param>
    /// <param name="throwOnUndefinedValue">A boolean representing whether to throw an exception if <paramref name="enumValue"/>
    /// is not a defined value of <typeparamref name="T"/>.</param>
    /// <returns></returns>
    /// <exception cref="System.ArgumentException">Thrown when <paramref name="throwOnUndefinedValue"/> is <c>true</c> and <paramref name="enumValue"/> is not a defined value of <typeparamref name="T"/>.</exception>
    public static string PrettyPrint<T>(this T enumValue, string flagSeparator = null, bool throwOnUndefinedValue = true) where T : struct, Enum {
      if (typeof(T)._hasAttribute<IgnorePrettyPrintAttribute>())
        return enumValue.ToString();

      if (typeof(T)._hasAttribute<FlagsAttribute>() && !Enum.IsDefined(typeof(T), enumValue))
        return _fromMultiFlags(enumValue, flagSeparator ?? Pretty.DefaultFlagSeparator, throwOnUndefinedValue);

      PrettyNameCache<T>._populateSingleValueCache();

      if (PrettyNameCache<T>._singleValueCache.TryGetValue(enumValue, out var cachedPrettyName))
        return cachedPrettyName;

      return _fromUndefined(enumValue, throwOnUndefinedValue);
    }
  }
}
