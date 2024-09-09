namespace Injact.Container.Injection;

public class Validator
{
    private const string TypeFactoryHasMultipleGenericTypesErrorMessage = "Factory \"{0}\" has multiple generic types which is not supported.";

    private readonly ILogger _logger;
    private readonly ContainerOptions _options;
    private readonly ObjectBindings _objectBindings;
    private readonly FactoryBindings _factoryBindings;
    private readonly HashSet<Type> _creatableTypes = new();

    public Validator(
        ILogger logger,
        ContainerOptions options,
        ObjectBindings objectBindings,
        FactoryBindings factoryBindings)
    {
        _logger = logger;
        _options = options;
        _objectBindings = objectBindings;
        _factoryBindings = factoryBindings;
    }

    public bool CanCreate(Type type)
    {
        if (_creatableTypes.Contains(type))
        {
            return true;
        }

        var parameters = ReflectionHelpers.GetConstructorParameters(type)
            .Where(s => s.GetCustomAttributes(typeof(InjectIgnoreAttribute), true).Length == 0);

        var canCreateType = parameters.All(s => CanInjectAsObject(s.ParameterType) || CanInjectAsFactory(s.ParameterType));
        if (canCreateType)
        {
            _creatableTypes.Add(type);
        }

        return canCreateType;
    }

    private bool CanInjectAsObject(Type type)
    {
        return _objectBindings.ContainsKey(type) || type.IsAssignableTo(typeof(ILogger));
    }

    private bool CanInjectAsFactory(Type type)
    {
        if (_factoryBindings.ContainsKey(type))
        {
            return true;
        }

        if (!_options.UseAutoFactories || !type.IsAssignableTo(typeof(IFactory)))
        {
            return false;
        }

        var genericTypes = type.GetGenericArguments();
        if (genericTypes.Length > 1)
        {
            _logger.LogWarning(string.Format(TypeFactoryHasMultipleGenericTypesErrorMessage, type.Name));
        }

        return CanCreate(genericTypes[0]);
    }
}