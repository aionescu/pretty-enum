namespace PrettyEnum;

using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;
using System.Text.RegularExpressions;

static class PrettyCache<TEnum> where TEnum : struct, Enum {
  internal static readonly bool hasIgnoreAttribute, hasFlagsAttribute;

  internal static readonly TEnum[] enumValues;
  internal static readonly string[] enumPrettyNames;
  internal static readonly Dictionary<TEnum, string> singleValueCache;
  internal static readonly Dictionary<string, TEnum> singleValueReverseCache;

  // https://stackoverflow.com/a/53710276
  internal static readonly Func<TEnum, TEnum, TEnum>? or;
  internal static readonly Func<TEnum, TEnum, bool>? and;

  static PrettyCache() {
    hasIgnoreAttribute = HasAttribute<IgnorePrettyPrintAttribute>(typeof(TEnum), out _);
    hasFlagsAttribute = HasAttribute<FlagsAttribute>(typeof(TEnum), out _);

    enumValues = (Enum.GetValues(typeof(TEnum)) as TEnum[])!;
    var enumValueNames = Enum.GetNames(typeof(TEnum));

    singleValueCache = new(enumValues.Length);
    singleValueReverseCache = new(enumValues.Length);

    if (hasIgnoreAttribute) {
      enumPrettyNames = enumValueNames;

      for (int i = 0; i < enumValues.Length; ++i) {
        var value = enumValues[i];
        var name = enumValueNames[i];
        singleValueCache[value] = name;
        singleValueReverseCache[name] = value;
      }
    } else {
      enumPrettyNames = new string[enumValues.Length];

      for (int i = 0; i < enumValues.Length; ++i) {
        var value = enumValues[i];
        var prettyName = PrettyPrintSingleValue(enumValueNames[i]);

        enumPrettyNames[i] = prettyName;
        singleValueCache[value] = prettyName;
        singleValueReverseCache[prettyName] = value;
      }
    }

    if (hasFlagsAttribute) {
      var t = Enum.GetUnderlyingType(typeof(TEnum));
      var a = Expression.Parameter(typeof(TEnum));
      var b = Expression.Parameter(typeof(TEnum));

      var convertA = Expression.Convert(a, t);
      var convertB = Expression.Convert(b, t);

      or = Expression.Lambda<Func<TEnum, TEnum, TEnum>>(Expression.Convert(Expression.Or(convertA, convertB), typeof(TEnum)), a, b).Compile();
      and = Expression.Lambda<Func<TEnum, TEnum, bool>>(Expression.NotEqual(Expression.And(convertA, convertB), Expression.Default(t)), a, b).Compile();
    }
  }

  private static bool HasAttribute<TAttribute>(MemberInfo memberInfo, [NotNullWhen(true)] out TAttribute? attribute) where TAttribute : Attribute {
    var attrs = (memberInfo.GetCustomAttributes(typeof(TAttribute), false) as TAttribute[])!;

    if (attrs.Length > 0) {
      attribute = attrs[0];
      return true;
    } else {
      attribute = default;
      return false;
    }
  }

  private static string FromCamelCase(string s) {
    var ro = RegexOptions.Compiled | RegexOptions.CultureInvariant;

    s = Regex.Replace(s, "([A-Z])([a-z])", " $1$2", ro);
    s = Regex.Replace(s, "([a-z])([A-Z])", "$1 $2", ro);
    s = Regex.Replace(s, "([0-9])([a-zA-Z])", " $1$2", ro);
    s = Regex.Replace(s, "([a-zA-Z])([0-9])", "$1 $2", ro);

    var words =
      s.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
      .Select(w => char.ToUpperInvariant(w[0]) + w.Substring(1).ToLowerInvariant());

    return string.Join(" ", words);
  }

  private static string PrettyPrintSingleValue(string rawName) {
    var field = typeof(TEnum).GetField(rawName)!;

    if (HasAttribute<IgnorePrettyPrintAttribute>(field, out _))
      return rawName;

    if (HasAttribute<PrettyNameAttribute>(field, out var attr) && !string.IsNullOrWhiteSpace(attr.PrettyName))
      return attr.PrettyName;

    if (HasAttribute<DescriptionAttribute>(field, out var descAttr) && !string.IsNullOrWhiteSpace(descAttr.Description))
      return descAttr.Description;

    var words = rawName.Split(new[] { '_' }, StringSplitOptions.RemoveEmptyEntries).Select(FromCamelCase);
    return string.Join(" ", words);
  }
}
