using System;

namespace Injact.Engine;

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
public class MappedAttribute : Attribute { }