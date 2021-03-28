namespace PrettyEnum {
  using System;
  using System.Collections.Generic;

  static class PrettyNameCache<T> where T: struct, Enum {
    internal static readonly T[] _enumValues = Enum.GetValues(typeof(T)) as T[];
    internal static readonly Dictionary<T, string> _singleValueCache = new Dictionary<T, string>();

    static PrettyNameCache() {
      foreach (var value in _enumValues)
        _singleValueCache[value] = EnumExtensions._fromSingleValue(value, false);
    }
  }
}
