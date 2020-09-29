namespace PrettyEnum {
  using System;
  using System.ComponentModel;
  using System.Linq;
  using System.Text.RegularExpressions;

  public static class EnumExtensions {
    public static string PrettyPrint<T>(this T @this, string flagSeparator = null, bool throwOnUndefinedValue = true) where T : struct, Enum {
      if (typeof(T).GetCustomAttributes(typeof(FlagsAttribute), false).Length > 0)
        return _fromFlags(@this, flagSeparator ?? Pretty.DefaultFlagSeparator, throwOnUndefinedValue);
      else
        return _prettyPrintNoFlags(@this, throwOnUndefinedValue);
    }

    private static string _prettyPrintNoFlags<T>(T value, bool throwOnUndefinedValue) where T : struct, Enum {
      if (!Enum.IsDefined(typeof(T), value))
        return _fromUndefined(value, throwOnUndefinedValue);

      if (PrettyNameCache<T>._cache.TryGetValue(value, out var prettyName))
        return prettyName;

      var rawName = value.ToString();

      if (typeof(T).GetCustomAttributes(typeof(IgnorePrettyPrintAttribute), false).Length > 0) {
        PrettyNameCache<T>._cache.Add(value, rawName);
        return rawName;
      }

      var field = typeof(T).GetField(rawName);

      if (field.GetCustomAttributes(typeof(IgnorePrettyPrintAttribute), false).Length > 0) {
        PrettyNameCache<T>._cache.Add(value, rawName);
        return rawName;
      }

      var prettyNameAttr = field.GetCustomAttributes(typeof(PrettyNameAttribute), false) as PrettyNameAttribute[];
      
      if (prettyNameAttr.Length > 0) {
        var attrName = prettyNameAttr[0].PrettyName;
        PrettyNameCache<T>._cache.Add(value, attrName);
        return attrName;
      }

      var descAttr = field.GetCustomAttributes(typeof(DescriptionAttribute), false) as DescriptionAttribute[];

      if (descAttr.Length > 0) {
        var attrDesc = descAttr[0].Description;
        PrettyNameCache<T>._cache.Add(value, attrDesc);
        return attrDesc;
      }

      var computedPrettyName = rawName.Contains("_") ? _fromSnakeCase(rawName) : _fromCamelCase(rawName);

      PrettyNameCache<T>._cache.Add(value, computedPrettyName);
      return computedPrettyName;
    }
        
    private static string _fromFlags<T>(T value, string flagSeparator, bool throwOnUndefinedValue) where T : struct, Enum {
      if (PrettyNameCache<T>._cache.TryGetValue(value, out var singleFlagName))
        return singleFlagName;
      else if (PrettyNameCache<T>._flagsCache.TryGetValue((value, flagSeparator), out var multiFlagName))
        return multiFlagName;

      var flags = Enum.GetValues(typeof(T)).Cast<T>().Where(f => value.HasFlag(f)).Select(f => _prettyPrintNoFlags(f, false));

      if (!flags.Any())
        return _fromUndefined(value, throwOnUndefinedValue);

      var prettyName = string.Join(flagSeparator, flags);
      
      PrettyNameCache<T>._flagsCache.Add((value, flagSeparator), prettyName);
      return prettyName;
    }

    private static string _fromUndefined<T>(T value, bool throwOnUndefinedValue) where T : struct, Enum {
      if (throwOnUndefinedValue)
        throw new ArgumentException($"Value {value} is not defined for the enum type {typeof(T).Name}.");
      else {
        if (PrettyNameCache<T>._cache.TryGetValue(value, out var cachedName))
          return cachedName;
          
        var undefString = $"Undefined[{value}]";
        PrettyNameCache<T>._cache.Add(value, undefString);
        return undefString;
      }
    }

    private static string _fromSnakeCase(string s) {
      return string.Join(" ", s.Split(new[] { "_" }, StringSplitOptions.RemoveEmptyEntries).Select(_capitalize));
    }
    
    private static string _capitalize(string s) {
      var lower = s.ToLower();
      
      if (char.IsLetter(lower[0]))
        return char.ToUpper(lower[0]) + lower.Substring(1);
      else
        return s;
    }
    
    private static string _regexReplace(this string @this, string pattern, string replacement) => Regex.Replace(@this, pattern, replacement, RegexOptions.Compiled);

    private static string _fromCamelCase(string s) {
      var replaced =
        s
        ._regexReplace("([A-Z])([a-z])", " $1$2")
        ._regexReplace("([a-z])([A-Z])", "$1 $2")
        ._regexReplace("([0-9])([a-z]|[A-Z])", " $1$2")
        ._regexReplace("([a-z]|[A-Z])([0-9])", "$1 $2");

      return string.Join(" ", replaced.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Select(_capitalize));
    }
  }
}
