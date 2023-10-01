using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Injact;

public static class ReflectionHelpers
{
    public static ConstructorInfo GetConstructor(Type type)
    {
        var constructors = type.GetConstructors();
        var defaultConstructor = constructors[0];

        foreach (var constructor in constructors)
        {
            if (!constructor.GetCustomAttributes(typeof(InjectAttribute), true).Any())
                continue;

            defaultConstructor = constructor;
            break;
        }

        return defaultConstructor;
    }

    public static IEnumerable<ParameterInfo> GetParameters(Type type)
    {
        var constructor = GetConstructor(type);
        return constructor.GetParameters();
    }

    public static FieldInfo[] GetAllFields(Type type)
    {
        Assert.IsNotNull(type, "Cannot get fields from null type!");
        var fields = new List<FieldInfo>();

        while (type != null)
        {
            fields.AddRange(
                type.GetFields(
                    System.Reflection.BindingFlags.Public |
                    System.Reflection.BindingFlags.NonPublic |
                    System.Reflection.BindingFlags.Instance |
                    System.Reflection.BindingFlags.DeclaredOnly));

            type = type.BaseType;
        }

        return fields.ToArray();
    }

    public static FieldInfo GetBackingField(Type type, string parameterName)
    {
        Assert.IsNotNull(type, "Cannot get field from null type!");
        Assert.IsNotNullOrWhitespace(parameterName, "Cannot get backing field from null or empty parameter name!");

        while (type != null)
        {
            var field = type.GetField($"<{parameterName}>k__BackingField", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (field != null)
                return field;

            type = type.BaseType;
        }

        return null;
    }
}