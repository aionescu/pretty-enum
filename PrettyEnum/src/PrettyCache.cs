namespace PrettyEnum;

using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

static class PrettyCache<TEnum> where TEnum : struct, Enum {
  internal static readonly bool hasFlagsAttribute;

  internal static readonly string[] enumPrettyNames;
  internal static readonly Dictionary<TEnum, string> singleValueCache;
  internal static readonly Dictionary<string, TEnum> singleValueReverseCache;

  // https://stackoverflow.com/a/53710276
  internal static readonly Func<TEnum, TEnum, TEnum>? or;
  internal static readonly Func<TEnum, TEnum, bool>? and;

  static PrettyCache() {
    var hasIgnoreAttr = typeof(TEnum).GetCustomAttribute<IgnorePrettyPrintAttribute>() is not null;
    var hasFlagsAttr = typeof(TEnum).GetCustomAttribute<FlagsAttribute>() is not null;

    var enumValues = (Enum.GetValues(typeof(TEnum)) as TEnum[])!;
    var enumValueNames = Enum.GetNames(typeof(TEnum));

    Dictionary<TEnum, string> cache = new(enumValues.Length);
    Dictionary<string, TEnum> reverseCache = new(enumValues.Length);

    if (hasIgnoreAttr) {
      for (var i = 0; i < enumValues.Length; ++i) {
        var value = enumValues[i];
        var name = enumValueNames[i];
        cache[value] = name;
        reverseCache[name] = value;
      }

      enumPrettyNames = enumValueNames;
    } else {
      var prettyNames = new string[enumValues.Length];

      for (var i = 0; i < enumValues.Length; ++i) {
        var value = enumValues[i];
        var prettyName = PrettyPrintSingleValue(enumValueNames[i]);

        prettyNames[i] = prettyName;
        cache[value] = prettyName;
        reverseCache[prettyName] = value;
      }

      enumPrettyNames = prettyNames;
    }

    if (hasFlagsAttr) {
      var t = Enum.GetUnderlyingType(typeof(TEnum));

      var a = Expression.Parameter(typeof(TEnum));
      var b = Expression.Parameter(typeof(TEnum));
      var at = Expression.Convert(a, t);
      var bt = Expression.Convert(b, t);

      or = Expression.Lambda<Func<TEnum, TEnum, TEnum>>(Expression.Convert(Expression.Or(at, bt), typeof(TEnum)), a, b).Compile();
      and = Expression.Lambda<Func<TEnum, TEnum, bool>>(Expression.NotEqual(Expression.And(at, bt), Expression.Default(t)), a, b).Compile();
    }

    hasFlagsAttribute = hasFlagsAttr;
    singleValueCache = cache;
    singleValueReverseCache = reverseCache;
  }

  private static string ToTitleCase(string rawName) {
    StringBuilder sb = new(rawName.Length);

    const byte UNDERSCORE = 0, OTHER = 1, LOWER = 2, UPPER = 3;
    byte last = UNDERSCORE;

    foreach (var c in rawName) {
      if (c == '_') {
        last = UNDERSCORE;
      } else if (char.IsUpper(c)) {
        if (last != UPPER)
          sb.Append(' ').Append(c);
        else
          sb.Append(char.ToLower(c));
        last = UPPER;
      } else if (char.IsLower(c)) {
        if ((last >> 1) == 0) // UNDERSCORE or OTHER
          sb.Append(' ').Append(char.ToUpper(c));
        else
          sb.Append(c);
        last = LOWER;
      } else {
        if (last != OTHER)
          sb.Append(' ');
        sb.Append(c);
        last = OTHER;
      }
    }

    return sb.Length == 0 ? rawName : sb.ToString(1, sb.Length - 1);
  }

  private static string PrettyPrintSingleValue(string rawName) {
    var field = typeof(TEnum).GetField(rawName)!;

    if (field.GetCustomAttribute<IgnorePrettyPrintAttribute>() is not null)
      return rawName;

    if (field.GetCustomAttribute<PrettyNameAttribute>() is { PrettyName: var prettyName } && !string.IsNullOrWhiteSpace(prettyName))
      return prettyName;

    if (field.GetCustomAttribute<DescriptionAttribute>() is { Description: var desc } && !string.IsNullOrWhiteSpace(desc))
      return desc;

    return ToTitleCase(rawName);
  }
}
