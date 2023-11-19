using System.Linq;
using System.Reflection;
using Injact.Profiling;

namespace Injact.Engine;

//TODO: Confirm this works when class/interface is assignable
internal class EditorValueMapper
{
    private readonly ILogger _logger;
    
    public EditorValueMapper(ILogger logger)
    {
        _logger = logger;
    }

    public void Map(object engineObject, object nativeObject)
    {
        var engineType = engineObject.GetType();
        var nativeType = nativeObject.GetType();
        
        const BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.DeclaredOnly;
        
        var engineTypeFields = engineType
            .GetFields(bindingFlags)
            .Where(s => s.GetCustomAttributes(typeof(MappedAttribute), true).Length > 0)
            .ToArray();
        
        var engineTypeProperties = engineType
            .GetProperties(bindingFlags)
            .Where(s => s.GetCustomAttributes(typeof(MappedAttribute), true).Length > 0)
            .ToArray();
        
        if (engineTypeFields.Length == 0 && engineTypeProperties.Length == 0)
            return;
        
        var nativeTypeFields = nativeType
            .GetFields(bindingFlags)
            .Where(s => s.GetCustomAttributes(typeof(MappedAttribute), true).Length > 0)
            .ToArray();
        
        var nativeTypeProperties = nativeType
            .GetProperties(bindingFlags)
            .Where(s => s.GetCustomAttributes(typeof(MappedAttribute), true).Length > 0)
            .ToArray();
        
        if (nativeTypeFields.Length == 0 && nativeTypeProperties.Length == 0)
        {
            _logger.LogWarning($"{engineType.Name} has values marked for mapping but the native class {nativeType.Name} does not have any matching values!");
            return;
        }
        
        var mappedCount = 0;

        foreach (var property in engineTypeFields)
        {
            var mappedProperty = nativeTypeFields.FirstOrDefault(s => s.Name == property.Name);
            if (mappedProperty == null)
                continue;
            
            //TODO: Check type is assignable

            mappedProperty.SetValue(nativeObject, property.GetValue(engineObject));
            mappedCount++;
        }
        
        foreach (var property in engineTypeProperties)
        {
            var mappedProperty = nativeTypeProperties.FirstOrDefault(s => s.Name == property.Name);
            if (mappedProperty == null)
                continue;

            mappedProperty.SetValue(nativeObject, property.GetValue(engineObject));
            mappedCount++;
        }
        
        if (mappedCount < nativeTypeProperties.Length + nativeTypeProperties.Length)
            _logger.LogWarning($"{engineType.Name} has {nativeTypeProperties.Length + nativeTypeProperties.Length} values marked for mapping but only {mappedCount} were mapped to the native class {nativeType}!");
    }
}