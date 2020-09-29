namespace PrettyEnum {
  using System;

  /// <summary>
  /// Signals that the enum or enum field it is applied to should not be pretty-printed,
  /// and should instead be stringified using the default <see cref="System.Enum.ToString()"/> implementation.
  /// </summary>
  [AttributeUsage(AttributeTargets.Enum | AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
  public sealed class IgnorePrettyPrintAttribute : Attribute { }
}
