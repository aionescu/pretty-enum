namespace PrettyEnum {
  using System;

  [AttributeUsage(AttributeTargets.Enum | AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
  public sealed class IgnorePrettyPrintAttribute : Attribute { }
}
