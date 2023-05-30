namespace PrettyEnum {
  using System;

  /// <summary>
  /// Signals that the enum field this attribute is applied to should not be pretty-printed,
  /// and should instead be stringified using the default <see cref="System.Enum.ToString()"/> implementation.
  /// <br/>
  /// If applied to an enum type, it will affect all of its values.
  /// </summary>
  [AttributeUsage(AttributeTargets.Enum | AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
  public sealed class IgnorePrettyPrintAttribute : Attribute { }
}
