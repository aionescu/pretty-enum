namespace PrettyEnum {
  using System;
  using System.Collections.Generic;
  using System.Linq;

  /// <summary>
  /// Static class that handles the caching of enum pretty names.
  /// </summary>
  /// <typeparam name="T">The type of the enum.</typeparam>
  public static class PrettyNameCache<T> where T: struct, Enum {
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

    /// <summary>
    /// Clears the cache of pretty names for single enum values/flags of type <typeparamref name="T"/>.
    /// </summary>
    public static void ClearSingleValueCache() {
      _isSingleValueCachePopulated = false;
      _singleValueCache.Clear();
    }

    /// <summary>
    /// Clears the cache of pretty names for enum values of type <typeparamref name="T"/> composed of multiple flags.
    /// </summary>
    public static void ClearMultiFlagsCache() => _multiFlagsCache.Clear();

    /// <summary>
    /// Clears all caches associated with the type <typeparamref name="T"/>.
    /// </summary>
    public static void Clear() {
      ClearSingleValueCache();
      ClearMultiFlagsCache();
    }
  }
}
