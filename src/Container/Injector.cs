using System;
using System.Linq;
using System.Reflection;

namespace Injact;

public class Injector
{
    private readonly DiContainer _container;

    public Injector(DiContainer container)
    {
        _container = container;
    }

    public void InjectInto(object target)
    {
        Assert.IsNotNull(target, "Cannot inject into null target!");

        InjectIntoFields(target);
        InjectIntoProperties(target);
        InjectIntoMethods(target);
    }

    private void InjectIntoFields(object requestingObject)
    {
        var fields = ReflectionHelpers.GetAllFields(requestingObject.GetType());

        var requiredFields = fields.Where(s => s.GetCustomAttributes(typeof(InjectAttribute), true).Length > 0);
        var optionalFields = fields.Where(s => s.GetCustomAttributes(typeof(InjectOptionalAttribute), true).Length > 0);

        var requestingType = requestingObject.GetType();

        foreach (var field in requiredFields)
            field.SetValue(requestingObject, _container.Resolve(field.FieldType, requestingType));

        foreach (var field in optionalFields)
            field.SetValue(requestingObject, _container.Resolve(field.FieldType, requestingType, false));
    }

    private void InjectIntoProperties(object requestingObject)
    {
        var properties = requestingObject
            .GetType()
            .GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        var requiredProperties = properties.Where(s => s.GetCustomAttributes(typeof(InjectAttribute), true).Length > 0);
        var optionalProperties = properties.Where(s => s.GetCustomAttributes(typeof(InjectOptionalAttribute), true).Length > 0);

        var requestingType = requestingObject.GetType();

        foreach (var property in requiredProperties)
            SetPropertyValue(property, requestingObject, requestingType);

        foreach (var property in optionalProperties)
            SetPropertyValue(property, requestingObject, requestingType, false);
    }

    private void SetPropertyValue(PropertyInfo property, object requestingObject, Type requestingType, bool throwOnNotFound = true)
    {
        var propertyType = property.PropertyType;
        var backingField = ReflectionHelpers.GetBackingField(requestingType, property.Name);

        Assert.IsNotNull(backingField, $"{nameof(InjectIntoProperties)} failed to find backing field for {property.Name} on {requestingType}!");

        backingField?.SetValue(requestingObject, _container.Resolve(propertyType, requestingType, throwOnNotFound));
    }

    private void InjectIntoMethods(object requestingObject)
    {
        var methods = requestingObject
            .GetType()
            .GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

        var requiredMethods = methods.Where(s => s.GetCustomAttributes(typeof(InjectAttribute), true).Length > 0);
        var optionalMethods = methods.Where(s => s.GetCustomAttributes(typeof(InjectOptionalAttribute), true).Length > 0);

        var requestingType = requestingObject.GetType();

        foreach (var method in requiredMethods)
        {
            var parameters = method
                .GetParameters()
                .Select(s => _container.Resolve(s.ParameterType, requestingType))
                .ToArray();

            method.Invoke(requestingObject, parameters);
        }

        foreach (var method in optionalMethods)
        {
            var parameters = method
                .GetParameters()
                .Select(s => _container.Resolve(s.ParameterType, requestingType, false))
                .ToArray();

            method.Invoke(requestingObject, parameters);
        }
    }
}