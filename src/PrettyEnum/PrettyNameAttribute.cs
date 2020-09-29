namespace PrettyEnum {
  using System;

  /// <summary>
  /// Signals that when pretty-printing the enum value it is applied to,
  /// the specified string should be returned.
  /// </summary>
  [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
  public sealed class PrettyNameAttribute : Attribute {
    /// <summary>
    /// The string to return when pretty-printing the enum value this attribute is applied to.
    /// </summary>
    public readonly string PrettyName;

    /// <summary>
    /// Constructs a new instance of the <see cref="PrettyNameAttribute"/> class with the specified <paramref name="prettyName"/>.
    /// </summary>
    /// <param name="prettyName">The string to return when pretty-printing the enum value this attribute is applied to</param>
    public PrettyNameAttribute(string prettyName) => PrettyName = prettyName;
  }
}
