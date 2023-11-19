using System;

namespace Injact;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class MappedAttribute : Attribute { }