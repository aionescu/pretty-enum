namespace PrettyEnum {
  using System;
  using System.Linq;
  using System.Reflection;

  static class ReflectionCache {
    internal static readonly MethodInfo _getNames = typeof(Pretty).GetMethod("GetNames", Array.Empty<Type>());
    internal static readonly MethodInfo _parse = typeof(Pretty).GetMethod("Parse", new[] { typeof(string), typeof(string ) });

    internal static readonly MethodInfo _prettyPrint =
      typeof(EnumExtensions)
      .GetMethods(BindingFlags.Public | BindingFlags.Static)
      .Where(m => m.Name == "PrettyPrint" && m.IsGenericMethodDefinition)
      .First();
  }
}
