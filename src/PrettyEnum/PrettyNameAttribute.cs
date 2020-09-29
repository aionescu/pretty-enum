namespace PrettyEnum {
  using System;
  
  [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
  public sealed class PrettyNameAttribute : Attribute {
    public string PrettyName { get; }

    public PrettyNameAttribute(string prettyName) => PrettyName = prettyName;
  }
}
