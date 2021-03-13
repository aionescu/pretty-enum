namespace PrettyEnum {
  using System;

  /// <summary>
  /// Signals that when the enum value this attribute is applied to is pretty-printed,
  /// The case of the words in its name should be preserved.
  /// <br/>
  /// If applied to an enum type, it will affect all of its values.
  /// </summary>
  [AttributeUsage(AttributeTargets.Enum | AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
  public sealed class PreserveCaseAttribute: Attribute { }
}
