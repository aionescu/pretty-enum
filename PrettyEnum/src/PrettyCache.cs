namespace PrettyEnum;

using System.ComponentModel;
using System.Linq.Expressions;

static class PrettyCache<TEnum> where TEnum : struct, Enum {
  internal static readonly bool hasIgnoreAttribute;
  internal static readonly bool hasFlagsAttribute;

  internal static readonly TEnum[] enumValues;
  internal static readonly string[] enumPrettyNames;
  internal static readonly Dictionary<TEnum, string> singleValueCache;
  internal static readonly Dictionary<string, TEnum> singleValueReverseCache;

  // https://stackoverflow.com/a/53710276
  internal static readonly Func<TEnum, TEnum, TEnum>? or;
  internal static readonly Func<TEnum, TEnum, bool>? and;

  static PrettyCache() {
    hasIgnoreAttribute = typeof(TEnum).HasAttribute<IgnorePrettyPrintAttribute>(out _);
    hasFlagsAttribute = typeof(TEnum).HasAttribute<FlagsAttribute>(out _);

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

  private static string Capitalize(string s)
    => char.ToUpperInvariant(s[0]) + s.Substring(1).ToLowerInvariant();

  private static string FromCamelCase(string s) {
    var replaced =
      s
      .RegexReplace("([A-Z])([a-z])", " $1$2")
      .RegexReplace("([a-z])([A-Z])", "$1 $2")
      .RegexReplace("([0-9])([a-zA-Z])", " $1$2")
      .RegexReplace("([a-zA-Z])([0-9])", "$1 $2");

    var words = replaced.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries).Select(Capitalize);
    return string.Join(" ", words);
  }

  private static string PrettyPrintSingleValue(string rawName) {
    var field = typeof(TEnum).GetField(rawName)!;

    if (field.HasAttribute<IgnorePrettyPrintAttribute>(out _))
      return rawName;

    if (field.HasAttribute<PrettyNameAttribute>(out var attr) && !string.IsNullOrWhiteSpace(attr.PrettyName))
      return attr.PrettyName;

    if (field.HasAttribute<DescriptionAttribute>(out var descAttr) && !string.IsNullOrWhiteSpace(descAttr.Description))
      return descAttr.Description;

    var words = rawName.Split(new[] { '_' }, StringSplitOptions.RemoveEmptyEntries).Select(FromCamelCase);
    return string.Join(" ", words);
  }
}
