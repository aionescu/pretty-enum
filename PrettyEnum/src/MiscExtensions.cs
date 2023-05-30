namespace PrettyEnum {
  using System;
  using System.Reflection;
  using System.Text.RegularExpressions;

  internal static class MiscExtensions {
    internal static bool HasAttribute<TAttribute>(this MemberInfo memberInfo, out TAttribute attribute) where TAttribute : Attribute {
      var attrs = memberInfo.GetCustomAttributes(typeof(TAttribute), false) as TAttribute[];

      if (attrs.Length > 0) {
        attribute = attrs[0];
        return true;
      } else {
        attribute = default;
        return false;
      }
    }

    internal static string RegexReplace(this string s, string pattern, string replacement)
      => Regex.Replace(s, pattern, replacement, RegexOptions.Compiled);
  }
}
