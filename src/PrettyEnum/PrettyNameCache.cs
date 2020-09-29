namespace PrettyEnum {
  using System;
  using System.Collections.Generic;
  using System.Linq;

  /// <summary>
  /// Static class that handles the caching of enum pretty names.
  /// </summary>
  /// <typeparam name="T">The type of the enum.</typeparam>
  public static class PrettyNameCache<T> where T : struct, Enum {
    internal static readonly Dictionary<T, string> _singleValueCache = new Dictionary<T, string>();
    internal static readonly Dictionary<(T EnumValue, string FlagSeparator), string> _multiFlagsCache = new Dictionary<(T, string), string>();

    internal static void _addToSingleValueCache(T value, string prettyName) => _singleValueCache[value] = prettyName;
    internal static void _addToMultiFlagsCache(T value, string flagSeparator, string prettyName) => _multiFlagsCache[(value, flagSeparator)] = prettyName;

    internal static bool _isSingleValueCached(T value, out string prettyName) => _singleValueCache.TryGetValue(value, out prettyName);
    internal static bool _isMultiFlagsCached(T value, string flagSeparator, out string prettyName) => _multiFlagsCache.TryGetValue((value, flagSeparator), out prettyName);

    internal static void _populateSingleValueCache() {
      foreach (var value in Enum.GetValues(typeof(T)).Cast<T>())
        if (!_singleValueCache.ContainsKey(value))
          _singleValueCache[value] = EnumExtensions._fromSingleValue(value, false);
    }

    /// <summary>
    /// Clears the cache of pretty names for single enum values/flags of type <typeparamref name="T"/>.
    /// </summary>
    public static void ClearSingleValueCache() => _singleValueCache.Clear();

    /// <summary>
    /// Clears the cache of pretty names for enum values of type <typeparamref name="T"/> composed of multiple flags.
    /// </summary>
    public static void ClearMultiFlagsCache() => _multiFlagsCache.Clear();
  }
}
