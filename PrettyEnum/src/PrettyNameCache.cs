namespace PrettyEnum {
  using System;
  using System.Collections.Generic;

  static class PrettyNameCache<T> where T: struct, Enum {
    internal static readonly T[] _enumValues = Enum.GetValues(typeof(T)) as T[];

    internal static bool _isSingleValueCachePopulated = false;
    internal static readonly Dictionary<T, string> _singleValueCache = new Dictionary<T, string>();
    internal static readonly Dictionary<(T EnumValue, string FlagSeparator), string> _multiFlagsCache = new Dictionary<(T, string), string>();

    internal static void _populateSingleValueCache() {
      if (!_isSingleValueCachePopulated) {
        foreach (var value in _enumValues)
          _singleValueCache[value] = EnumExtensions._fromSingleValue(value, false);

        _isSingleValueCachePopulated = true;
      }
    }
  }
}
