namespace PrettyEnum {
  using System;
  using System.Collections.Generic;
  
  internal static class PrettyNameCache<T> where T : struct, Enum {
    internal static readonly Dictionary<T, string> _cache = new Dictionary<T, string>();
    internal static readonly Dictionary<(T EnumValue, string FlagSeparator), string> _flagsCache = new Dictionary<(T, string), string>();
  }
}
