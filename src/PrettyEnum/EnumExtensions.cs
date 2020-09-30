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
      ? char.ToUpper(s[0]) + s.Substring(1).ToLower()
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

    private static string _fromSnakeCase(string s, bool preserveCase) {
      var words = s.Split(new[] { '_' }, StringSplitOptions.RemoveEmptyEntries);
      return string.Join(" ", preserveCase ? words : words.Select(_capitalize));
    }

    private static string _fromMultiFlags<T>(T value, string flagSeparator, bool throwOnUndefinedValue) where T : struct, Enum {
      var flags = PrettyNameCache<T>._enumValues.Where(f => value.HasFlag(f)).Select(f => _fromSingleValue(f, false));

      if (!flags.Any())
        return _fromUndefined(value, throwOnUndefinedValue);

      return string.Join(flagSeparator, flags);
    }

    internal static string _fromSingleValue<T>(T value, bool throwOnUndefinedValue) where T : struct, Enum {
      if (!Enum.IsDefined(typeof(T), value))
        return _fromUndefined(value, throwOnUndefinedValue);

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

      return
        rawName.Contains("_")
        ? _fromSnakeCase(rawName, preserveCase)
        : _fromCamelCase(rawName, preserveCase);
    }

    internal static (bool IsMultiFlags, string PrettyName) _prettyPrintNoCache<T>(this T @this, string flagSeparator = null, bool throwOnUndefinedValue = true) where T : struct, Enum {
      if (typeof(T)._hasAttribute<IgnorePrettyPrintAttribute>())
        return (false, @this.ToString());

      if (typeof(T)._hasAttribute<FlagsAttribute>() && !Enum.IsDefined(typeof(T), @this))
        return (true, _fromMultiFlags(@this, flagSeparator ?? Pretty.DefaultFlagSeparator, throwOnUndefinedValue));
      else
        return (false, _fromSingleValue(@this, throwOnUndefinedValue));
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
      flagSeparator = flagSeparator ?? Pretty.DefaultFlagSeparator;

      if (PrettyNameCache<T>._isSingleValueCached(enumValue, out var singleValuePrettyName))
        return singleValuePrettyName;
      else if (typeof(T)._hasAttribute<FlagsAttribute>() && PrettyNameCache<T>._isMultiFlagsCached(enumValue, flagSeparator, out var multiFlagsPrettyName))
        return multiFlagsPrettyName;
      else {
        var (isMultiFlags, prettyName) = _prettyPrintNoCache<T>(enumValue, flagSeparator, throwOnUndefinedValue);

        if (isMultiFlags)
          PrettyNameCache<T>._addToMultiFlagsCache(enumValue, flagSeparator, prettyName);
        else
          PrettyNameCache<T>._addToSingleValueCache(enumValue, prettyName);

        return prettyName;
      }
    }
  }
}
